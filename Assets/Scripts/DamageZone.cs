using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D col) // 停留在区域内
    {
        var controller = col.GetComponent<RubyController>();
        if (controller == null){return;}
        controller.ChangeHealth(-1);
    }
}
