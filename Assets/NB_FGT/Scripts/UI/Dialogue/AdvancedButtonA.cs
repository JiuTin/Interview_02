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
    //默认包括键盘，手柄上下选择的时候，如果是鼠标进入需要自己重写
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _forntRing.Fade(1, 0.1f, null);
        //设置光标的位置
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
        //由于注音的效果，字体的显示是协程。   在AdvancedText里的SetText函数里添加协程为空不使用，   ?.Invoke();
        text.StartCoroutine(text.SetText(content, AdvancedText.DisplayType.Default));
    }
}