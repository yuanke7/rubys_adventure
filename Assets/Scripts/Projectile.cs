using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public float maxAlive = 5.0f;
    public float speed = 10.0f;
    // Start is called before the first frame update
    // void Start() 不会被调用，而是在开始后的下一帧
    private void Awake() // 与start不同的是， 在类被实例化创建时会立即调用awake
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 超时销毁
        if (maxAlive >= 0)
        {
            maxAlive -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {

        // 或者距离过大时销毁
        // if (transform.position.magnitude > 20.0f)
        // {
        //     Destroy(gameObject);
        // }
    }

    public void Launch(Vector2 director, float force)
    {
        _rigidbody2D.AddForce(force * director);
    }

    public void LaunchByTransform(Vector2 director)
    {
        this.GetComponent<Rigidbody>().velocity = new Vector2(director.x * speed, director.y * speed); 
    }
    
    public void OnCollisionEnter2D(Collision2D col)
    {
        
        // Debug.Log($"齿轮击中了目标:{col.gameObject.name}");
        // if (col.collider.GetComponent<Tilemap>() == null)  // 碰到的不为tile map 则销毁
        // {
            // Destroy(gameObject);  // 销毁当前组件所在的游戏对象(飞弹本身)
        // }
        Destroy(gameObject); 
        var obj = col.collider.GetComponent<EnemyController>();
        if (obj != null)
        {
            obj.Fix();  // 修复机器人
            // Destroy(col.gameObject); // 销毁击中的目标    
        }
    }
}
