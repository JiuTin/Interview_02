using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Button
{
    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(OnClickEvent);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();   // 执行，进入到OnSelect回调里面
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        UIManager.SetCurrentSelectable(this);
    }
    protected virtual void OnClickEvent()
    {

    }
    protected int _index;
    public Action<int> OnConfirm; // int 参数代表自身的下标序号，动画执行完成时调用
    public virtual void Init(string content, int index, Action<int> onConfirmEvent)
    {
        _index = index;
        OnConfirm += onConfirmEvent;
    }
    public void Confirm()
    {
        OnConfirm(_index);
    }
}
