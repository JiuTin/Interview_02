using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GGG.Tool;
using GGG.Tool.Singleton;
public class GameEventManager : SingletonNonMono<GameEventManager>
{
    private interface IEventHelp
    { }

    private class EventHelp : IEventHelp
    {
        private Action _action;
        public EventHelp(Action action)
        {
            _action = action;
        }

        public void AddCall(Action action)
        {
            _action += action;
        }

        public void Call()
        {
            _action?.Invoke();
        }

        public void Remove(Action action)
        {
            _action -= action;
        }
    }
    private class EventHelp<T> : IEventHelp
    {
        private Action<T> _action;
        public EventHelp(Action<T> action)
        {
            _action = action;
        }

        public void AddCall(Action<T> action)
        {
            _action += action;
        }

        public void Call(T value)
        {
            _action?.Invoke(value);
        }

        public void Remove(Action<T> action)
        {
            _action -= action;
        }
    }
    private class EventHelp<T1,T2> : IEventHelp
    {
        private Action<T1, T2> _action;
        public EventHelp(Action<T1, T2> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1, T2> action)
        {
            _action += action;
        }

        public void Call(T1 value1,T2 value2)
        {
            _action?.Invoke(value1,value2);
        }

        public void Remove(Action<T1, T2> action)
        {
            _action -= action;
        }
    }
    private class EventHelp<T1, T2, T3> : IEventHelp
    {
        private Action<T1, T2,T3> _action;
        public EventHelp(Action<T1, T2,T3> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1, T2,T3> action)
        {
            _action += action;
        }

        public void Call(T1 value1, T2 value2,T3 value3)
        {
            _action?.Invoke(value1, value2,value3);
        }

        public void Remove(Action<T1, T2,T3> action)
        {
            _action -= action;
        }
    }
    private class EventHelp<T1, T2,T3,T4,T5> : IEventHelp
    {
        private Action<T1, T2, T3, T4, T5> _action;
        public EventHelp(Action<T1, T2, T3, T4, T5> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1, T2, T3, T4, T5> action)
        {
            _action += action;
        }

        public void Call(T1 value1, T2 value2,T3 value3,T4 value4,T5 value5)
        {
            _action?.Invoke(value1, value2, value3, value4, value5);
        }

        public void Remove(Action<T1, T2, T3, T4, T5> action)
        {
            _action -= action;
        }
    }

    //事件中心处理
    Dictionary<string, IEventHelp> _eventCenter = new Dictionary<string, IEventHelp>();


    /// <summary>
    /// 事件监听
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">回调函数</param>
    public void AddEventListening(string eventName, Action action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp)?.AddCall(action);
        }
        else
        {
            _eventCenter.Add(eventName, new EventHelp(action));
        }
    }
    public void AddEventListening<T>(string eventName, Action<T> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T>)?.AddCall(action);
        }
        else
        {
            _eventCenter.Add(eventName, new EventHelp<T>(action));
        }
    }
    public void AddEventListening<T1,T2>(string eventName, Action<T1, T2> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2>)?.AddCall(action);
        }
        else
        {
            _eventCenter.Add(eventName, new EventHelp<T1, T2>(action));
        }
    }
    public void AddEventListening<T1, T2,T3>(string eventName, Action<T1, T2,T3> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2,T3>)?.AddCall(action);
        }
        else
        {
            _eventCenter.Add(eventName, new EventHelp<T1, T2,T3>(action));
        }
    }
    public void AddEventListening<T1, T2,T3,T4,T5>(string eventName, Action<T1, T2,T3,T4,T5> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2,T3,T4,T5>)?.AddCall(action);
        }
        else
        {
            _eventCenter.Add(eventName, new EventHelp<T1, T2,T3,T4,T5>(action));
        }
    }


    /// <summary>
    /// 调用事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void CallEvent(string eventName)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp)?.Call();
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法执行该事件");
        }
    }
    public void CallEvent<T>(string eventName,T value1)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T>)?.Call(value1);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法执行该事件");
        }
    }
    public void CallEvent<T1,T2>(string eventName,T1 value1,T2 value2)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1,T2>)?.Call(value1,value2);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法执行该事件");
        }
    }
    public void CallEvent<T1, T2,T3>(string eventName, T1 value1, T2 value2,T3 value3)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2,T3>)?.Call(value1, value2,value3);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法执行该事件");
        }
    }
    public void CallEvent<T1, T2, T3, T4, T5>(string eventName, T1 value1, T2 value2,T3 value3,T4 value4,T5 value5)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2, T3, T4, T5>)?.Call(value1, value2,value3,value4,value5);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法执行该事件");
        }
    }


    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="action">回调函数</param>
    public void RemoveEvent(string eventName,Action action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp)?.Remove(action);
        }
        else
        { 
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法删除该事件");
        }
    }
    public void RemoveEvent<T>(string eventName, Action<T> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T>)?.Remove(action);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法删除该事件");
        }
    }
    public void RemoveEvent<T1,T2>(string eventName, Action<T1, T2> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2>)?.Remove(action);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法删除该事件");
        }
    }
    public void RemoveEvent<T1, T2,T3>(string eventName, Action<T1, T2,T3> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2,T3>)?.Remove(action);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法删除该事件");
        }
    }
    public void RemoveEvent<T1, T2,T3,T4,T5>(string eventName, Action<T1, T2,T3,T4,T5> action)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
        {
            (e as EventHelp<T1, T2,T3,T4,T5>)?.Remove(action);
        }
        else
        {
            DevelopmentToos.WTF($"当前未找到{eventName}的事件，无法删除该事件");
        }
    }
}
