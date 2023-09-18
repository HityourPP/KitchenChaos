using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounter : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    private GameObject virtualGameObject;
    private void Start()
    {        
        virtualGameObject = transform.GetChild(0).gameObject;   //要显示的物体
        PlayerController.Instance.OnSelectedCounterChanged += SelectedCounterChanged;//为创建的包含类参数的事件添加执行函数
    }

    private void SelectedCounterChanged(object sender, PlayerController.OnSelectedCounter e)
    {
        if (e.selectedCounter == baseCounter)//当事件中的参数与赋值的相同时
        {
            virtualGameObject.SetActive(true); //显示选中状态
        }
        else
        {
            virtualGameObject.SetActive(false);
        }
    }
}
