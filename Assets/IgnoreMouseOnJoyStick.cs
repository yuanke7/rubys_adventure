using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreMouseOnJoyStick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 当摇杆游戏对象进入其它游戏对象的触发器时调用
    void OnTriggerEnter2D(Collider2D other)
    {
        // 设置鼠标的点击状态为 false，以防止触发鼠标点击事件
        Input.simulateMouseWithTouches = false;
    }

    // 当摇杆游戏对象离开其它游戏对象的触发器时调用
    void OnTriggerExit2D(Collider2D other)
    {
        // 设置鼠标的点击状态为 true，以便在摇杆游戏对象离开后可以再次触发鼠标点击事件
        Input.simulateMouseWithTouches = true;
    }
}
