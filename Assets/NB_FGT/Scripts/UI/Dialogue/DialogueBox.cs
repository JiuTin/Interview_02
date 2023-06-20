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

    private bool _interactable;      //�ɽ���
    private bool _printFinished;     //�Ƿ��ӡ��
    private bool _canQuickShow;   //�ɷ���ٴ�ӡ
    private bool _autoNext;       //�Զ���һ��

    private bool CanQuickShow => !_printFinished && _canQuickShow;
    private bool CanNext => _printFinished;
    public Action<bool> OnNext;   // bool����������һ�仰�Ƿ�ǿ��ֱ����ʾ
    private void Awake()
    {
        _content.OnFinished = PrintFinished;
    }
    void Update()
    {
        if (_interactable) UpdateInput();
    }
    //һ�仰��ӡ��󣬵��õĻص�
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
        
        //������
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
        //�ո��
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
    //д��UI������
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
    /// ����Ի����ݵ��Ի���
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
