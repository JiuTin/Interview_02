using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool.Singleton;
using System;
public class TimeManager : Singleton<TimeManager>
{
    //note:��ʼ��ʱ��Ҫ�ȴ���һЩ��ʱ����
    //1.��һ�����������������еĿ��м�ʱ��
    //2.��һ�������������浱ǰ���ڹ����еļ�ʱ��
    //3.���µ�ǰ�����еļ�ʱ��
    //4.��ĳ����ʱ��������ɺ�������Ҫ�������յ����м�ʱ���ļ�����
    [SerializeField] private int _initMaxTimerCount;
    private Queue<GameTimer> _notWorkerTimer = new Queue<GameTimer>();
    private List<GameTimer> _workeringTimer = new List<GameTimer>();

    private void Start()
    {
        InitTimerManager();
    }

    private void Update()
    {
        UpdateWorkeringTimer();
    }
    private void InitTimerManager()
    {
        for (int i = 0; i < _initMaxTimerCount; i++)
        {
            CreateTimer();
        }
    }
    private void CreateTimer()
    {
        var timer = new GameTimer();
        //���浽���м�ʱ���ļ�����ȥ
        _notWorkerTimer.Enqueue(timer);
    }

    public void TryGetOneTimer(float time, Action task)
    {
        if (_notWorkerTimer.Count == 0)
        {
            CreateTimer();
            var timer = _notWorkerTimer.Dequeue();
            timer.StartTimer(time, task);
            _workeringTimer.Add(timer);
        }
        else
        {
            var timer = _notWorkerTimer.Dequeue();
            timer.StartTimer(time, task);
            _workeringTimer.Add(timer);
        }
    }
    private void UpdateWorkeringTimer()
    {
        if (_workeringTimer.Count == 0) return;
        for (int i = 0; i < _workeringTimer.Count; i++)
        {
            if (_workeringTimer[i].GetTimerState() == TimerState.WORKERING)
            {
                //�жϵ�ǰ�����ļ�ʱ���Ĺ���״̬�Ƿ��ڹ�����
                _workeringTimer[i].UpdateTimer();
            }
            else
            {
                //˵�����������
                _notWorkerTimer.Enqueue(_workeringTimer[i]);
                _workeringTimer[i].ResetTimer();
                _workeringTimer.Remove(_workeringTimer[i]);
            }
        }
        
    }
}