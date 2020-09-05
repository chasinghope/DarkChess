using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 资源加载模块
/// 1. 异步加载
/// 2. 委托和lamabda表达式
/// 3. 协程
/// 4. 泛型
/// </summary>
public class ResMgr : InstanceNull<ResMgr>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T">加载的资源类型</typeparam>
    /// <param name="name">加载的资源名字</param>
    /// <returns>加载后的资源</returns>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
        {
            // 如果对象是一个GameObject 把它实例化后在返回出去，外部直接调用即可
            return GameObject.Instantiate(res);
        }
        else
            return res;

    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">加载的资源类型</typeparam>
    /// <param name="name">加载的资源名字</param>
    /// <param name="action">加载后的资源</param>
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        // 开启异步加载协程
        MonoManager.Instance.StartCoroutine(ReallyLoadAsync(name, callback));
    }


    /// <summary>
    /// 真正的协同程序函数用于开启异步加载对应的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if( r.asset is GameObject)
        {
            callback(GameObject.Instantiate(r.asset) as T);
        }
        else
        {
            callback(r.asset as T);
        }
    
    }
}
