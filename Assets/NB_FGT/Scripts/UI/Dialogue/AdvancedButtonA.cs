using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdvancedButtonA : AdvancedButton
{
    Widget _forntRing;
    Animator _animator;
    protected override void Awake()
    {
        base.Awake();
        _forntRing = transform.Find("Select").GetComponent<Widget>();
        _animator = GetComponent<Animator>();
    }
    //Ĭ�ϰ������̣��ֱ�����ѡ���ʱ���������������Ҫ�Լ���д
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _forntRing.Fade(1, 0.1f, null);
        //���ù���λ��
       // UIManager.UpdateCursorA(transform.position);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _forntRing.Fade(0, 0.25f, null);
    }
    private static readonly int Click = Animator.StringToHash("Click");
    protected override void OnClickEvent()
    {
        base.OnClickEvent();
        _animator.SetTrigger(Click);
        //UIManager.ClickCursorA();
    }
    public override void Init(string content, int index, Action<int> onConfirmEvent)
    {
        base.Init(content, index, onConfirmEvent);
        AdvancedText text = GetComponentInChildren<AdvancedText>();
        //����ע����Ч�����������ʾ��Э�̡�   ��AdvancedText���SetText���������Э��Ϊ�ղ�ʹ�ã�   ?.Invoke();
        text.StartCoroutine(text.SetText(content, AdvancedText.DisplayType.Default));
    }
}