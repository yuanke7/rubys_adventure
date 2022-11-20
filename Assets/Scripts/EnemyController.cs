using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 3.0f;
    public bool vertical = true;
    public float maxMoveTime = 5.0f;
    public float moveTimer;
    public int direction = 1;
    public int damageToPlayer = 1;
    public bool broken = true;
    public float attackArea = 5.0f;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    Random ran = new Random();
    private static readonly int Fixed = Animator.StringToHash("Fixed");

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        moveTimer = maxMoveTime;
    }

    // Update is called once per frame
    private void Update()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer > 0) return;
        vertical = ran.Next(0,2) == 1;  // 实现随机垂直或水平移动
        Debug.Log(vertical);
        direction = -direction;
        moveTimer = maxMoveTime;
    }

    private void FixedUpdate()
    {
        if (!broken){return;}  // 修复后停止移动
        var position = _rigidbody2D.position;
        FollowPlayer(position);
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
        // _rigidbody2D.MovePosition(position); // 如果使用这一行  玩家碰到机器人后会被推开且持续移动
        _rigidbody2D.position = position; // 这一行则不会， 尽量使用这种移动方式
    }

    private void OnCollisionEnter2D(Collision2D obj)
    {
        var player = obj.gameObject.GetComponent<RubyController>();
        var tilemap = obj.gameObject.GetComponent<Tilemap>();
        if (player)
        {
            player.ChangeHealth(-damageToPlayer);
        }
        // 撞击到不可穿越物体立刻回头
        if (tilemap)
        {
            direction = -direction;
        }
    }

    public void Fix()
    {
        broken = false;
        // 让机器人不会被碰撞
        _rigidbody2D.simulated = false; // 刚体属性直接消除
        _animator.SetBool(Fixed, true);
    }

    public void FollowPlayer(Vector2 position)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerPosition = player.gameObject.GetComponent<Rigidbody2D>().position;
        var distanceX = position.x - playerPosition.x;
        var distanceY = position.y - playerPosition.y;
        var distance = Vector2.Distance(position, playerPosition);
        if (distance>attackArea){return;}
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
