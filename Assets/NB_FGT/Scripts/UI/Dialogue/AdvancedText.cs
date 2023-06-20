using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 知识点
/// 1.textPreprocessor (接口类):预处理器
/// </summary>

public class AdvancedText : TextMeshProUGUI
{
    public enum DisplayType
    {
        Default,      //默认
        Fading,      //淡入
        Typing       //逐字
    }
    private Widget _widget;         //实现淡入淡出类

    private int _typingIndex;     //当前打印的下标
    private float _defaultInterval = 0.2f;  //默认停顿时间

    public Action OnFinished;
    private Coroutine _typingCoroutine;

    private AdvancedTextPreprocessor SelfPreprocessor =>
        (AdvancedTextPreprocessor)textPreprocessor;
    /// <summary>
    protected override void Awake()
    {
        base.Awake();
        _widget = GetComponent<Widget>();
    }
    //构造函数，使用AdvancedTextPreprocessor
    public AdvancedText()
    {
        textPreprocessor = new AdvancedTextPreprocessor();   //实现接口类，需要转化
    }

    /// 初始化
    /// </summary>
    public void Initialise()
    {
        SetText("");
        ClearRubyText();
    }
    //淡入
    public void Disappear(float duration = 0.2f)
    {
        _widget.Fade(0, duration, null);
    }
    /// <summary>
    /// 逐字效果：
    ///     1.先遍历字符，将单个字符透明度为0
    ///     2.开启FadeInCharacter协程，将每个字符逐渐显现
    /// 停顿效果：
    ///     3.yield return WaitForSencondsRealtime( result ),  ==从IntervalDictionary里获得停顿时间
    ///4.实现OnFinished()回调函数
    /// </summary>
    /// <returns></returns>
    IEnumerator Typing()
    {
        ForceMeshUpdate(); //强制网格更新
        //1.所有字符透明度 为0
        for (int i = 0; i < m_characterCount; i++)    //m_characterCount文本字符数量(已Preprocessor更新)
        {
            SetSingleCharacterAlpha(i, 0);
        }
        _typingIndex = 0;
        //
        while (_typingIndex < m_characterCount)    //下标小于所有文字的数量
        {
            //SetSingleCharacterAlpha(_typingIdnex,255);
            //字符可见才开启协程
            StartCoroutine(FadeInCharacter(_typingIndex));
            if (SelfPreprocessor.IntervalDictionary.TryGetValue(_typingIndex, out float result))
            {
                yield return new WaitForSecondsRealtime(result);     //TODO
            }
            else
            {
                yield return new WaitForSecondsRealtime(_defaultInterval);
            }
            _typingIndex++;
        }
        OnFinished.Invoke();
    }

    /// <summary>
    /// 渐变效果的实现：
    /// 1.设置注解：
    ///     
    /// 2.如果duration小于0，直接显示；大于0，不太理解
    /// </summary>
    /// <param name="index"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator FadeInCharacter(int index, float duration = 0.5f)
    {
        //设置注解
        if (SelfPreprocessor.TryGetRubyStarFrom(index, out RubyData data))
        {
            SetRubyText(data);
        }
        if (duration <= 0)
        {
            SetSingleCharacterAlpha(index, 255);
        }
        else
        {
            //渐变效果的实现
            float timer = 0;
            while (timer < duration)
            {
                //获得不超过duration的值，且unscaleDeltatime不受时间暂停的影响。
                timer = Mathf.Min(duration, timer + Time.unscaledDeltaTime);
                SetSingleCharacterAlpha(index, (byte)(255 * timer / duration));
                yield return null;
            }
        }
    }

    /// <summary>
    /// 设置单个字符的阿尔法值，范围为0~255
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newAlpha"></param>
    private void SetSingleCharacterAlpha(int index, byte newAlpha)
    {
        //当前文本的所有charcter信息
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible)
        {
            return;
        }
        //材质实例的索引
        int matIndex = charInfo.materialReferenceIndex;
        //顶点颜色的索引
        int vertIndex = charInfo.vertexIndex;
        //修改4个顶点的阿尔法
        for (int i = 0; i < 4; i++)
        {
            textInfo.meshInfo[matIndex].colors32[vertIndex + i].a = newAlpha;
        }
        //更新顶点数据
        UpdateVertexData();
    }

    /// <summary>
    /// 实例化注解，并设置注解对象的位置为：（开始位置+结束位置）/2
    /// </summary>
    /// <param name="data"></param>
    private void SetRubyText(RubyData data)
    {
        //实例化注解的预制体对象
        //TODO    可以使用对象池优化
        GameObject obj = Resources.Load<GameObject>("RubyText");
        GameObject ruby = Instantiate(obj, transform);
        ruby.GetComponent<TextMeshProUGUI>().SetText(data.Content);
        ruby.GetComponent<TextMeshProUGUI>().color = textInfo.characterInfo[data.StartIndex].color;
        ruby.transform.localPosition = (textInfo.characterInfo[data.StartIndex].topLeft + textInfo.characterInfo[data.EndIndex].topRight) / 2;
    }

    //加载出全部的注解
    private void SetAllRubyText()
    {
        foreach (var item in SelfPreprocessor._rubyList)
        {
            SetRubyText(item);
        }
    }
    //清除注解
    private void ClearRubyText()
    {
        foreach (var item in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (item != this)
            {
                Destroy(item.gameObject);
            }
        }
    }
    /// <summary>
    /// 快速显示，可以在输入系统的时候调用(如：点击鼠标时，快速加载)
    /// </summary>
    public void QuickShowRemaining()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            for (; _typingIndex < m_characterCount; _typingIndex++)
            {
                StartCoroutine(FadeInCharacter(_typingIndex));
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="type"></param>
    /// <param name="fadingDuratioin"></param>
    /// <returns></returns>
    public IEnumerator SetText(string content, DisplayType type, float fadingDuratioin = 0.2f)
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        ClearRubyText();
        SetText(content);     //自己的SetText,设置文本
        yield return null;   //等待一帧
        switch (type)
        {
            case DisplayType.Default:
                _widget.RenderOpacity = 1;
                SetAllRubyText();
                OnFinished?.Invoke();
                break;
            case DisplayType.Fading:
                _widget.Fade(1, fadingDuratioin,OnFinished.Invoke);
                SetAllRubyText();
                break;
            case DisplayType.Typing:
                _widget.Fade(1, fadingDuratioin, null);
                _typingCoroutine = StartCoroutine(Typing());
                break;
            default:
                break;

        }
    }
}
