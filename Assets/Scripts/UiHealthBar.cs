using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHealthBar : MonoBehaviour
{
    // 血条对象 其他对象能获取但不能更改  设置为静态表示被所有实例共享
    public static UiHealthBar Instance { get; private set; }

    public Image mask;
    // 设置变量 记录遮罩初始长度
    private float _originalSize;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        // 更改遮罩层的宽度  根据参数更改
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _originalSize * value);
    }
}
