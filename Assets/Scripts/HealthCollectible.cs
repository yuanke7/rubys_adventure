using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D obj)
    {
        Debug.Log($"Object <{obj.name}> has enter the HealthCollectible area.");
        var controller = obj.GetComponent<RubyController>();
        if (controller == null) return;
        Debug.Log(controller.Health);
        controller.ChangeHealth(1);  
        Destroy(gameObject);  // 销毁当前使用此脚本的对象
    }
}
