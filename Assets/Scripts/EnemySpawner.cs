using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public float enemySpawnInterval = 2.0f;
    private float _enemySpawnTimer;
    public int maxRobotNum = 100;
    private int _generatedNum = 0;
    private int _curRobotNum = 0;
    public int maxAliveNum = 5;
    public GameObject robot;
    private Vector2 _position;
    // Start is called before the first frame update
    void Start()
    {
        _enemySpawnTimer = enemySpawnInterval;
        _position = gameObject.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        _curRobotNum = GameObject.FindGameObjectsWithTag("Robot").Length;
        if (_enemySpawnTimer > 0)
        {
            _enemySpawnTimer -= Time.deltaTime;
        }
        else
        {
            if (_curRobotNum < maxAliveNum && _generatedNum < maxRobotNum)
            {
                var robotControl = robot.GetComponent<EnemyController>();
                var ran = Random.Range(2, 5);
                robotControl.speed = ran;
                Instantiate(robot, _position - Vector2.up * 1.0f, Quaternion.identity);  // 初始化对象
                _generatedNum += 1;
                _enemySpawnTimer = enemySpawnInterval;
            }
        }
    }
}
