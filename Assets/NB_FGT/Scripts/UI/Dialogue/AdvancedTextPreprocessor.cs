using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/// <summary>
/// ע������
/// </summary>
public class RubyData
{
    public int StartIndex { get; }
    public int EndIndex { get; set; }
    public string Content { get; set; }
    public RubyData(int startIndex, string content)
    {
        StartIndex = startIndex;
        Content = content;
        EndIndex = StartIndex;
    }
}
/// <summary>
/// ���ı����ݽ���Ԥ����
/// </summary>
public class AdvancedTextPreprocessor : ITextPreprocessor
{
    public Dictionary<int, float> IntervalDictionary;

    //ע��
    public List<RubyData> _rubyList;

    public bool TryGetRubyStarFrom(int index,out RubyData data)
    {
        data = new RubyData(0,"");
        foreach (var item in _rubyList)
        {
            if (item.StartIndex == index)
            {
                data = item;
                return true;
            }
        }
        return false;
    }
    public AdvancedTextPreprocessor()
    {
        IntervalDictionary = new Dictionary<int, float>();
        _rubyList = new List<RubyData>();
    }
    /// <summary>
    /// �ı�������������ݻᾭ���ú����Ĵ���
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string PreprocessText(string text)
    {
        IntervalDictionary.Clear();
        _rubyList.Clear();
        string processingText = text;
        //.     ���������ַ�
        //*     ����ǰһ���ַ�����0�λ���
        //+     ����ǰһ���ַ�����һ�λ���
        //?     ����ǰһ���ַ�����0�λ�һ��
        string pattern = "<.*?>";   //  .*����ƥ����̵Ľ��
        Match match = Regex.Match(processingText, pattern);
        while (match.Success)
        {
            string label = match.Value.Substring(1, match.Length - 2);   //<>�ڵ�����
            if (float.TryParse(label, out float result))
            {
                IntervalDictionary[match.Index - 1] = result; //ƥ�䵽���±�(<)-1
            }
            else if (Regex.IsMatch(label, "^r=.+"))   //ƥ��<r=��ע�⡱></r>
            {
                //���r= ������ַ���
                _rubyList.Add(new RubyData(match.Index, label.Substring(2)));
            }
            else if (label == "/r")
            {
                if (_rubyList.Count > 0)
                {
                    _rubyList[_rubyList.Count - 1].EndIndex = match.Index - 1;
                }
            }
            //ɾ��<....>
            processingText = processingText.Remove(match.Index, match.Length);
            //<sorite=dsadsa>  ��������*���
            if (Regex.IsMatch(label, "^sprite =.+"))
            {
                processingText = processingText.Insert(match.Index,"*");         //ռλ����ֹ�ӳٵ��õ�ʱ�����
            }
            //����ƥ��
            match = Regex.Match(processingText, pattern);
        }
        processingText = text;
        pattern = @"(<(\d+)(\.\d+)?>)|(</r>)|(<r=.*?>)";      //������ʽ��+=>ǰһ���ַ�һ�λ���
                                                                       //          ��()=>���������ڵ����ݿ���ʡ��
        processingText = Regex.Replace(processingText, pattern,"");  //������pattern���滻�ɡ���
        return processingText;
    }
}
