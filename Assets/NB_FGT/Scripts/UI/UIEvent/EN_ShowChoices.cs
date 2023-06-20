using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceData
{
    public string Content;
    public bool bQuickLocate;
    //��Ч
}
[CreateAssetMenu(fileName ="Node", menuName ="Event/Message/Show Choices")]
public class EN_ShowChoices : EventNodeBase
{
    public ChoiceData[] datas;
    public SequenceEventExecutor[] executors;
    public int defaultSelectIndex = 0;
    public override void Init(Action<bool> onFinishedEvent)
    {
        base.Init(onFinishedEvent);
        //��ÿ��ִ������ʼ��
        foreach (var item in executors)
        {
            if (null != item)
            {
                item.Init(OnFinished);
            }
        }
    }
    public override void Execute()
    {
        base.Execute();
        //��ʾ����ѡ��
        UIManager.CreateDialogueChocies(datas, OnChoiceConfirm, defaultSelectIndex);
    }
    private void OnChoiceConfirm(int index)
    {
        if (index < executors.Length && null != executors[index])
        {
            executors[index].Execute();
        }
        else
        {
            OnFinished(true);
        }
    }
}
