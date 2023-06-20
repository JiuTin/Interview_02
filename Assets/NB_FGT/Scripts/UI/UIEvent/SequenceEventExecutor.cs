using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Executor",menuName ="Event/Sequence Executor")]
public class SequenceEventExecutor : ScriptableObject
{
    public Action<bool> OnFinished; //bool��������ִ����ִ���Ƿ�ɹ�
    private int _index;
    public EventNodeBase[] nodes;
    public void Init(Action<bool> onFinishedEvent)
    {
        _index = 0;
        foreach (EventNodeBase item in nodes)
        {
            if (item != null)
            {
                item.Init(OnNodeFinished);
            }
        }
        OnFinished = onFinishedEvent;
    }
    //˳��ڵ��Ƿ�ִ����ɣ��Ǿ�ִ����һ���ڵ�
    private void OnNodeFinished(bool success)
    {
        if (success)
        {
            ExecuteNextNode();
        }
        else
        {
            OnFinished(false);
        }
    }

    private void ExecuteNextNode()
    {
        if (_index < nodes.Length)
        {
            if (nodes[_index].state == NodeState.Waiting)
            {
                nodes[_index].Execute();          //ִ�нڵ��¼����±�++
                _index++;
            }
        }
        else                       //���нڵ�ִ�����
        {
            OnFinished(true);
        }
    }
    public void Execute()
    {
        _index = 0;
        ExecuteNextNode();
    }
}
