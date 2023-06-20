using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image _background;
    [SerializeField] private Widget _widget;
    [SerializeField] private TextMeshProUGUI _speaker;
    [SerializeField] private AdvancedText _content;
    //[SerializeField] private Widget _nextCursorWidget;
    //[SerializeField] private Animator _nextCursorAnimator;
    private static readonly int _click = Animator.StringToHash("Click");
    [Header("Configs")]
    [SerializeField] private Sprite[] _backgroundStyles;

    private bool _interactable;      //可交互
    private bool _printFinished;     //是否打印完
    private bool _canQuickShow;   //可否快速打印
    private bool _autoNext;       //自动下一句

    private bool CanQuickShow => !_printFinished && _canQuickShow;
    private bool CanNext => _printFinished;
    public Action<bool> OnNext;   // bool参数代表下一句话是否强制直接显示
    private void Awake()
    {
        _content.OnFinished = PrintFinished;
    }
    void Update()
    {
        if (_interactable) UpdateInput();
    }
    //一句话打印完后，调用的回调
    private void PrintFinished()
    {
        if (_autoNext)
        {
            _interactable = false;
            OnNext(false);
        }
        else
        {
            _interactable = true;
            _printFinished = true;
          //  _nextCursorWidget.Fade(1, 0.2f, null);
        }
    }

    private void UpdateInput()
    {
        
        //鼠标左键
        if (GameInputManager.MainInstance.LAttack)
        {
            if (CanQuickShow)
            {
                _content.QuickShowRemaining();
                PrintFinished();
            }
            else if (CanNext)
            {
                _interactable = false;
               // _nextCursorAnimator.SetTrigger(_click);
               //_nextCursorWidget.Fade(0, 0.5f, null);
                OnNext(true);
            }
        }
        //空格键
        else if (GameInputManager.MainInstance.Interaction)
        {
            if (CanNext)
            {
                _interactable = false;
              //  _nextCursorAnimator.SetTrigger(_click);
              //  _nextCursorWidget.Fade(0, 0.5f, null);
                OnNext(false);
            }
        }
    }
    //写到UI基类里
    public void Open(Action<bool> nextEvent, int boxStyle = 0)
    {
        OnNext = nextEvent;
        _background.sprite = _backgroundStyles[boxStyle];
        //if (gameObject.activeSelf)
        //{
        gameObject.SetActive(true);
        _widget.RenderOpacity = 0;
        _widget.Fade(1, 0.2f, null);
        _speaker.SetText("");
        _content.Initialise();
    //}
      //  _nextCursorWidget.RenderOpacity = 0;
        OnNext(false);
    }
    public void Close(Action onClosed)
    {
        _widget.Fade(0, 0.4f, () =>
        {
            onClosed?.Invoke();
            gameObject.SetActive(false);
        });
    }
    /// <summary>
    /// 输出对话内容到对话框
    /// </summary>
    /// <param name="content"></param>
    /// <param name="speaker"></param>
    /// <param name="needTyping"></param>
    /// <param name="autoNext"></param>
    /// <param name="canQuickShow"></param>
    /// <returns></returns>
    public IEnumerator PrintDialogue(string content, string speaker, bool needTyping = true, bool autoNext = false, bool canQuickShow = true)
    {
        _interactable = false;
        _printFinished = false;
        if (_content.text !="")
        {
            _content.Disappear();
            yield return new WaitForSecondsRealtime(0.3f);
        }
        _canQuickShow = canQuickShow;
        _autoNext = autoNext;
        _speaker.SetText(speaker);
        if (needTyping)
        {
            _interactable = true;
            _content.StartCoroutine(_content.SetText(content, AdvancedText.DisplayType.Typing));
        }
        else
        {
            _content.StartCoroutine(_content.SetText(content, AdvancedText.DisplayType.Fading));
        }
    }
}
