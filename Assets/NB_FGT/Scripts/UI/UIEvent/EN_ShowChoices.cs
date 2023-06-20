using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceData
{
    public string Content;
    public bool bQuickLocate;
    //音效
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
        //对每个执行器初始化
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
        //显示所有选项
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
