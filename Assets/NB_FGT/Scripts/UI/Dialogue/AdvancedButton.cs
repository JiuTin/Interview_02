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
        Select();   // ִ�У����뵽OnSelect�ص�����
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
    public Action<int> OnConfirm; // int ��������������±���ţ�����ִ�����ʱ����
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
