using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono的管理者
/// 1. 事件
/// 2. 生命周期函数
/// 3. 协程
/// 
/// 说明：让没有继承Mono的类可以开启协程，可以
/// Update更新，统一管理Update
///
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
            updateEvent();
            
    }


    /// <summary>
    /// 给外部提供的   添加帧更新事件的函数
    /// </summary>
    /// <param name="fun">外部相当于update的函数</param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }


    /// <summary>
    /// 给外部提供的   移除帧更新事件的函数
    /// </summary>
    /// <param name="fun">外部相当于update的函数</param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
