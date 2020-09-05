using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

public class MonoManager : InstanceNull<MonoManager>
{
    private MonoController controller;

	public MonoManager()
	{
		GameObject obj = new GameObject("MonoController");
		controller = obj.AddComponent<MonoController>();

	}


    #region Update 功能
    /// <summary>
    /// 给外部提供的   添加帧更新事件的函数
    /// </summary>
    /// <param name="fun">外部相当于update的函数</param>
    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }


    /// <summary>
    /// 给外部提供的   移除帧更新事件的函数
    /// </summary>
    /// <param name="fun">外部相当于update的函数</param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }
    #endregion


    #region 协程功能

    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }


    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName, value);
    }

    public void StopAllCoroutines()
    {
        controller.StopAllCoroutines();
    }

    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }

    public void StopCoroutine(Coroutine routine)
    {
        controller.StopCoroutine(routine);
    }

    public void StopCoroutine(string methodName)
    {
        controller.StopCoroutine(methodName);
    }



    #endregion


}
