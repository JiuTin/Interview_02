using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;
	//当前选中的控件
	private Selectable _currentSelectable;

	[SerializeField] private DialogueBox _dialogueBox;
	private GameObject _prbButtonA;

	//设置选项的选中光标
	//[SerializeField] private RectTransform _cursorA;
	private static readonly int Click = Animator.StringToHash("Click");


	private void Awake()
	{
		if (_instance != null)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		_instance._prbButtonA = Resources.Load<GameObject>("UI/Button/ButtonA");
		DontDestroyOnLoad(gameObject);
	}
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (_instance._currentSelectable != null)
            {
                _instance._currentSelectable.Select();
            }
        }
    }
	public static void SetCurrentSelectable(Selectable obj)
	{
		_instance._currentSelectable = obj;
	}


	public static void OpenDialogueBox(Action<bool> onNextEvent, int boxStyle = 0)
	{
		_instance._dialogueBox.Open(onNextEvent, boxStyle);
		TP_CameraController.LockMouse(false);
	}
	public static void CloseDialogueBox()
	{
		_instance._dialogueBox.Close(null);
		TP_CameraController.LockMouse(true);
	}
	//输出内容
	public static void PrintDialogue(DialogueData data)
	{
		_instance._dialogueBox.StartCoroutine(_instance._dialogueBox.PrintDialogue(
			data.Content,
			data.Speaker,
			data.NeedTyping,
			data.AutoNext,
			data.CanQuickShow
		));
	}

	//public static void UpdateCursorA(Vector3 position)
	//{
	//	if (!_instance._cursorA.gameObject.activeSelf)
	//	{
	//		_instance._cursorA.gameObject.SetActive(true);
	//	}
	//	_instance._cursorA.position = position;  //已经调整过选项的中心点Pivot
	//}
	//public static void ClickCursorA()
	//{
	//	_instance._cursorA.GetComponentInChildren<Animator>().SetTrigger(Click);
	//}

	[SerializeField] private ChoicePanel _choicePanel;
	public static void CreateDialogueChocies(ChoiceData[] datas, Action<int> onConfirmEvent, int defaultSelectIndex)
	{
		for (int i=0; i < datas.Length; i++)
		{
			AdvancedButton button = Instantiate(_instance._prbButtonA).GetComponent<AdvancedButtonA>();
			button.gameObject.name = "ButtonA" + i;
			button.Init(datas[i].Content, i, onConfirmEvent);
			_instance._choicePanel.AddButton(button);
		}
		_instance._choicePanel.Open(defaultSelectIndex);
	}


	//public static void HideCursorA()
	//{
	//	_instance._cursorA.gameObject.SetActive(false);
	//}
}
