using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem smokeEffect;
    public float speed = 3.0f;
    public bool vertical = true;
    public float maxMoveTime = 5.0f;
    public float moveTimer;
    public int direction = 1;
    public int damageToPlayer = 1;
    public bool broken = true;
    public float attackArea = 5.0f;
    private Vector2 _stuckedPosition;
    private bool _stucked = false;
    public float maxStuckedTime = 2.0f;
    private float _stuckedTimer; 
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    public bool autoAttack = false;
    Random ran = new Random();
    private static readonly int Fixed = Animator.StringToHash("Fixed");
    // sound
    private AudioSource _audioSource;
    public AudioClip hitRobot;
    public AudioClip hitRobotStrong;
    public AudioClip robotFixed;
    public AudioClip robotWalk;
    // health
    public int maxHealth = 10;
    private int _curHealth;
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        moveTimer = maxMoveTime;
        _audioSource.clip = robotWalk;
        _audioSource.Play();
        _curHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer > 0) return;
        vertical = ran.Next(0,2) == 1;  // 实现随机垂直或水平移动
        // Debug.Log(vertical);
        direction = -direction;
        moveTimer = maxMoveTime;
    }

    private void FixedUpdate()
    {
        if (!broken){return;}  // 修复后停止移动
        var position = _rigidbody2D.position;  // 当前位置

        if (_stuckedTimer <= 0)
        {
            // 在寻路过程中遇到障碍物则调整
            if (_stuckedPosition.normalized == position.normalized)
            {   // direction
                // up [true 1]   down [true -1]   left [false -1]  right [false 1] 
                Debug.Log($"{position.normalized} {_stuckedPosition.normalized}");
                vertical = !vertical;
                _stucked = true;
                _stuckedTimer = maxStuckedTime;
            }
            else
            {
                _stucked = false;
            }
        }
        else
        {
            _stuckedTimer-= Time.deltaTime;
        }

        _stuckedPosition = position;
        
        // 跟随玩家
        if (!_stucked && autoAttack)
        {
            FollowPlayer(position);
        }
        
        // 行走
        if (vertical)
        {
            _animator.SetFloat("MoveX", 0);
            _animator.SetFloat("MoveY", direction);
            position.y += Time.deltaTime * speed * direction;
        }
        else
        {
            _animator.SetFloat("MoveX", direction);
            _animator.SetFloat("MoveY", 0);
            position.x += Time.deltaTime * speed * direction;
        }
        _rigidbody2D.MovePosition(position); // 这一行不会
        // _rigidbody2D.position = position; // 如果使用这一行  玩家碰到机器人后会被推开且持续移动
    }

    private void OnCollisionEnter2D(Collision2D obj)
    {
        var player = obj.gameObject.GetComponent<RubyController>();
        var tilemap = obj.gameObject.GetComponent<Tilemap>();
        if (player)
        {
            player.ChangeHealth(-damageToPlayer);
        }
        // 撞击到不可穿越物体则回头
        if (tilemap)
        {
            direction = -direction;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<RubyController>();
        if (player)
        {
            player.ChangeHealth(-damageToPlayer);
        }
    }

    private void StopSound()
    {
        // 停止播放行走声音
        _audioSource.Stop();
    }
    
    public void Fix()
    {
        if (_curHealth == 0)
        {
            _audioSource.PlayOneShot(robotFixed);
            broken = false;
            // 让机器人不会被碰撞
            _rigidbody2D.simulated = false; // 刚体属性直接消除
            _animator.SetBool(Fixed, true);
            // 删除烟雾效果
            // Destroy(smokeEffect);
            smokeEffect.Stop();  // 停止产生新的粒子  需要系统学习粒子系统 todo
            Invoke(nameof(StopSound), 1f); //延时方法  
        }else
        {
            _audioSource.PlayOneShot(hitRobot);
            _audioSource.PlayOneShot(hitRobotStrong);
            _curHealth--;
        }

    }

    public void FollowPlayer(Vector2 position)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null){return;}
        var playerPosition = player.gameObject.GetComponent<Rigidbody2D>().position;
        var distanceX = position.x - playerPosition.x;
        var distanceY = position.y - playerPosition.y;
        var distance = Vector2.Distance(position, playerPosition);
        if (distance>attackArea){return;}   // 控制机器人警觉范围
        
        // 根据玩家位置改变机器人行进方向 在没有卡住的情况下
        if (Mathf.Abs(distanceX) > Math.Abs(distanceY))
        {
            vertical = false;
            direction = distanceX > 0 ? -1 : 1;
        }
        else
        {
            vertical = true;
            direction = distanceY > 0 ? -1 : 1;
        }
    }
}
