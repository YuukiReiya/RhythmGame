using System.Collections.Generic;
using UnityEngine;
public abstract class SingletonObjectPool<T> : Yuuki.SingletonMonoBehaviour<SingletonObjectPool<T>>
{
    //proteced param
    [Header("Parameter")]
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected uint poolCount;
    protected List<GameObject> poolList;

    /// <summary>
    /// 初期化
    /// 
    /// オブジェクトを作った後にアクティブを切っているので、入れておきたい場合はオーバーライド
    /// </summary>
    protected virtual void Setup()
    {
        poolList = new List<GameObject>();
        for (int i = 0; i < poolCount; ++i)
        {
            poolList.Add(CreateNewInstance());
        }
        foreach (var it in poolList)
        {
            it.SetActive(false);
        }
    }

    /// <summary>
    /// 子オブジェクトの全取得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    protected virtual void GetAllChild<T>() where T : MonoBehaviour
    {
        foreach (Transform it in transform)
        {
            T obj;
            if (!it.TryGetComponent<T>(out obj)) { continue; }
            poolList.Add(obj.gameObject);
        }
    }

    /// <summary>
    /// プールするオブジェクトの作成
    /// </summary>
    protected virtual void CreatePoolObjects()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < poolCount; ++i)
        {
            var inst = CreateNewInstance();
            inst.name = prefab.name + ":" + (childCount + i + 1);
        }
    }

    /// <summary>
    /// オブジェクトの取得
    /// </summary>
    /// <returns></returns>
    public virtual GameObject GetObject()
    {
        foreach (var it in poolList)
        {
            //  Activeなものは使用されている
            if (it.activeSelf) { continue; }
            //  Deactiveなものを見つけたらActiveにして返す。
            it.SetActive(true);
            return it;
        }
        //  リストの中に使えるインスタンスがなかったため生成
        var instance = CreateNewInstance();
        poolList.Add(instance);
        return instance;
    }

    /// <summary>
    /// インスタンスの生成
    /// </summary>
    /// <returns></returns>
    private GameObject CreateNewInstance()
    {
        var instance = Instantiate(prefab);
        instance.name = prefab.name + ":" + (poolList.Count + 1);
        instance.transform.SetParent(this.transform);
        return instance;
    }
}
