using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 3.0f;
    public bool vertical = true;
    public float maxMoveTime = 5.0f;
    public float moveTimer;
    public int direction = 1;
    public int damageToPlayer = 1;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

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
        direction = -direction;
        moveTimer = maxMoveTime;
    }

    private void FixedUpdate()
    {
        var position = _rigidbody2D.position;
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
        if (player)
        {
            player.ChangeHealth(-damageToPlayer);
        }
    }
}
