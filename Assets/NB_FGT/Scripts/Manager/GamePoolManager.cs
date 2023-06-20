using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool.Singleton;
using GGG.Tool;
public class GamePoolManager : Singleton<GamePoolManager>
{
    //1.��Ҫ�Ȼ�������Ҫ����Ķ���
    //2.����
    //3.���ⲿ���Ի�ȡ����
    //4.���ն���
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
        _poolItemParent = new GameObject("�����Item�ĸ�����");
        _poolItemParent.transform.SetParent(this.transform);
        InitPool();
    }
    private void InitPool()
    {
        //1.�ж��ⲿ�����ǲ��ǿ�
        if (_configPoolItem.Count == 0) return;
        for (var i = 0; i < _configPoolItem.Count; i++)
        {
            for (var j = 0; j < _configPoolItem[i].InitMaxCount; j++)
            {
                var item = Instantiate(_configPoolItem[i].Item);
                item.transform.SetParent(_poolItemParent.transform);
                item.SetActive(false);
                //�жϳ�������û�д����������
                if (!_poolCenter.ContainsKey(_configPoolItem[i].ItemName))
                {
                    //�����ǰ�������û��һ����ATKSound�ĳ��ӣ���ô��Ҫ����һ��
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
            //�ж���û��һ����name�ĳ��Ӵ���
            var item = _poolCenter[name].Dequeue();
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);

        }
        else
        {
            //TODO --������û��ʱ�����Դ����µĳ��ӣ��ο�InitPool
            //DevelopmentToos.WTF("��ǰ����ĳ��ӿ��ܲ�����");
        }
    }
    public GameObject TryGetPoolItem(string name)
    {
        if (_poolCenter.ContainsKey(name))
        {
            //�ж���û��һ����name�ĳ��Ӵ���
            var item = _poolCenter[name].Dequeue();
            item.SetActive(true);
            _poolCenter[name].Enqueue(item);
            return item;

        }
        //DevelopmentToos.WTF("��ǰ����ĳ��ӿ��ܲ�����");
        return null;
    }
}
