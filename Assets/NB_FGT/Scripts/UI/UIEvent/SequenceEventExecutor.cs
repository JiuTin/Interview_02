using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Executor",menuName ="Event/Sequence Executor")]
public class SequenceEventExecutor : ScriptableObject
{
    public Action<bool> OnFinished; //bool参数代表执行器执行是否成功
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
    //顺序节点是否执行完成，是就执行下一个节点
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
                nodes[_index].Execute();          //执行节点事件并下标++
                _index++;
            }
        }
        else                       //所有节点执行完成
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
