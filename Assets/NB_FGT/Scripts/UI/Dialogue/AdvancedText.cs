using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ֪ʶ��
/// 1.textPreprocessor (�ӿ���):Ԥ������
/// </summary>

public class AdvancedText : TextMeshProUGUI
{
    public enum DisplayType
    {
        Default,      //Ĭ��
        Fading,      //����
        Typing       //����
    }
    private Widget _widget;         //ʵ�ֵ��뵭����

    private int _typingIndex;     //��ǰ��ӡ���±�
    private float _defaultInterval = 0.2f;  //Ĭ��ͣ��ʱ��

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
    //���캯����ʹ��AdvancedTextPreprocessor
    public AdvancedText()
    {
        textPreprocessor = new AdvancedTextPreprocessor();   //ʵ�ֽӿ��࣬��Ҫת��
    }

    /// ��ʼ��
    /// </summary>
    public void Initialise()
    {
        SetText("");
        ClearRubyText();
    }
    //����
    public void Disappear(float duration = 0.2f)
    {
        _widget.Fade(0, duration, null);
    }
    /// <summary>
    /// ����Ч����
    ///     1.�ȱ����ַ����������ַ�͸����Ϊ0
    ///     2.����FadeInCharacterЭ�̣���ÿ���ַ�������
    /// ͣ��Ч����
    ///     3.yield return WaitForSencondsRealtime( result ),  ==��IntervalDictionary����ͣ��ʱ��
    ///4.ʵ��OnFinished()�ص�����
    /// </summary>
    /// <returns></returns>
    IEnumerator Typing()
    {
        ForceMeshUpdate(); //ǿ���������
        //1.�����ַ�͸���� Ϊ0
        for (int i = 0; i < m_characterCount; i++)    //m_characterCount�ı��ַ�����(��Preprocessor����)
        {
            SetSingleCharacterAlpha(i, 0);
        }
        _typingIndex = 0;
        //
        while (_typingIndex < m_characterCount)    //�±�С���������ֵ�����
        {
            //SetSingleCharacterAlpha(_typingIdnex,255);
            //�ַ��ɼ��ſ���Э��
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
    /// ����Ч����ʵ�֣�
    /// 1.����ע�⣺
    ///     
    /// 2.���durationС��0��ֱ����ʾ������0����̫���
    /// </summary>
    /// <param name="index"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator FadeInCharacter(int index, float duration = 0.5f)
    {
        //����ע��
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
            //����Ч����ʵ��
            float timer = 0;
            while (timer < duration)
            {
                //��ò�����duration��ֵ����unscaleDeltatime����ʱ����ͣ��Ӱ�졣
                timer = Mathf.Min(duration, timer + Time.unscaledDeltaTime);
                SetSingleCharacterAlpha(index, (byte)(255 * timer / duration));
                yield return null;
            }
        }
    }

    /// <summary>
    /// ���õ����ַ��İ�����ֵ����ΧΪ0~255
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newAlpha"></param>
    private void SetSingleCharacterAlpha(int index, byte newAlpha)
    {
        //��ǰ�ı�������charcter��Ϣ
        TMP_CharacterInfo charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible)
        {
            return;
        }
        //����ʵ��������
        int matIndex = charInfo.materialReferenceIndex;
        //������ɫ������
        int vertIndex = charInfo.vertexIndex;
        //�޸�4������İ�����
        for (int i = 0; i < 4; i++)
        {
            textInfo.meshInfo[matIndex].colors32[vertIndex + i].a = newAlpha;
        }
        //���¶�������
        UpdateVertexData();
    }

    /// <summary>
    /// ʵ����ע�⣬������ע������λ��Ϊ������ʼλ��+����λ�ã�/2
    /// </summary>
    /// <param name="data"></param>
    private void SetRubyText(RubyData data)
    {
        //ʵ����ע���Ԥ�������
        //TODO    ����ʹ�ö�����Ż�
        GameObject obj = Resources.Load<GameObject>("RubyText");
        GameObject ruby = Instantiate(obj, transform);
        ruby.GetComponent<TextMeshProUGUI>().SetText(data.Content);
        ruby.GetComponent<TextMeshProUGUI>().color = textInfo.characterInfo[data.StartIndex].color;
        ruby.transform.localPosition = (textInfo.characterInfo[data.StartIndex].topLeft + textInfo.characterInfo[data.EndIndex].topRight) / 2;
    }

    //���س�ȫ����ע��
    private void SetAllRubyText()
    {
        foreach (var item in SelfPreprocessor._rubyList)
        {
            SetRubyText(item);
        }
    }
    //���ע��
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
    /// ������ʾ������������ϵͳ��ʱ�����(�磺������ʱ�����ټ���)
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
        SetText(content);     //�Լ���SetText,�����ı�
        yield return null;   //�ȴ�һ֡
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
