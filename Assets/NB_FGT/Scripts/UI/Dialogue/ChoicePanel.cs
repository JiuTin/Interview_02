using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoicePanel : Widget
{
    private List<AdvancedButton> _buttons = new List<AdvancedButton>();
    public void AddButton(AdvancedButton button)
    {
        _buttons.Add(button);
        button.transform.SetParent(transform);
        button.transform.localScale = Vector3.one;
        button.onClick.AddListener(DisableAllButtons);
        button.OnConfirm += (_) => { Close(); };  //_:弃元参数
    }
    //禁用所有按钮
    private void DisableAllButtons()
    {
        foreach (var item in _buttons)
        {
            item.enabled = false;
        }
    }
    public void Open(int defaultSelectIndex, float duration = 0.2f)
    {
        RenderOpacity = 0.0f;
        Fade(1f, duration, () => {
            if (defaultSelectIndex < _buttons.Count)
            {
                _buttons[defaultSelectIndex].Select();
            }
            else
            {
                _buttons[0].Select();
            }
        });
    }
    public void Close(float duration = 0.2f)
    {
        UIManager.SetCurrentSelectable(null);
        Fade(0f, duration, () =>
        {
            foreach (var item in _buttons)
            {
                Destroy(item.gameObject);
            }
            _buttons.Clear();
            //UIManager.HideCursorA();
        });
    }

}