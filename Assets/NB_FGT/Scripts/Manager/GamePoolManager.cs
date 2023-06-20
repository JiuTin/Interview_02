using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool.Singleton;
using GGG.Tool;
public class GamePoolManager : Singleton<GamePoolManager>
{
    //1.需要先缓存我们要缓存的对象
    //2.缓存
    //3.让外部可以获取对象
    //4.回收对象
    [System.Serializable]
    private class PoolItem
    {
        public string ItemName;
        public GameObject Item;
        public int InitMaxCount;
    }

    [SerializeField]private List<PoolItem> _configPoolItem = new List<PoolItem>();
    private Dictionary<string, Queue<GameObject>> _poolCenter = new Dictionary<string, Queue<GameObject>>();
    private GameObject _poolItemParent;
    private void Start()
    {
        _poolItemParent = new GameObject("对象池Item的父对象");
        _poolItemParent.transform.SetParent(this.transform);
        InitPool();
    }
    private void InitPool()
    {
        //1.判断外部配置是不是空
        if (_configPoolItem.Count == 0) return;
        for (var i = 0; i < _configPoolItem.Count; i++)
        {
            for (var j = 0; j < _configPoolItem[i].InitMaxCount; j++)
            {
                var item = Instantiate(_configPoolItem[i].Item);
                item.transform.SetParent(_poolItemParent.transform);
                item.SetActive(false);
                //判断池子中有没有存在这个对象
                if (!_poolCenter.ContainsKey(_configPoolItem[i].ItemName))
                {
                    //如果当前对象池中没有一个角ATKSound的池子，那么需要创建一个
                    _poolCenter.Add(_configPoolItem[i].ItemName, new Queue<GameObject>());
                    _poolCenter[_configPoolItem[i].ItemName].Enqueue(item);
                }
                else
                {
                    _poolCenter[_configPoolItem[i].ItemName].Enqueue(item);
                }
            }
            //DevelopmentToos.WTF(_configPoolItem.Count);
            //DevelopmentToos.WTF(_poolCenter["ATKSound"].Count);
        }
    }
    public void TryGetPoolItem(string name, Vector3 position, Quaternion rotation)
    {
        if (_poolCenter.ContainsKey(name))
        {
            //判断有没有一个叫name的池子存在
            var item = _poolCenter[name].Dequeue();
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);

        }
        else
        {
            //TODO --池子里没有时，可以创建新的池子，参考InitPool
            //DevelopmentToos.WTF("当前申请的池子可能不存在");
        }
    }
    public GameObject TryGetPoolItem(string name)
    {
        if (_poolCenter.ContainsKey(name))
        {
            //判断有没有一个叫name的池子存在
            var item = _poolCenter[name].Dequeue();
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);
            return item;

        }
        //DevelopmentToos.WTF("当前申请的池子可能不存在");
        return null;
    }
}
