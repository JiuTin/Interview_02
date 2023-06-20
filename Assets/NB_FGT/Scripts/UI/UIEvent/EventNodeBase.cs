using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NodeState
{
    Waiting,
    Executing,
    Finished
}
public class EventNodeBase : ScriptableObject
{
    protected Action<bool> OnFinished;
    [HideInInspector] public NodeState state;
    public virtual void Init(Action<bool> onFinishedEvent)
    {
        OnFinished = onFinishedEvent;
        OnFinished += OnFinishedBase;
        state = NodeState.Waiting;
    }
    public virtual void Execute()
    {
        if (state != NodeState.Waiting) return;
        state = NodeState.Executing;
    }
    protected virtual void OnFinishedBase(bool flag)
    { 
        if (flag)
        {
            state = NodeState.Waiting;
        }
    }
}