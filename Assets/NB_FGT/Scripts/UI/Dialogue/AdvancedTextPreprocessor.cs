using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/// <summary>
/// 注解内容
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
/// 对文本内容进行预处理
/// </summary>
public class AdvancedTextPreprocessor : ITextPreprocessor
{
    public Dictionary<int, float> IntervalDictionary;

    //注解
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
    /// 文本框内输入的内容会经过该函数的处理
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string PreprocessText(string text)
    {
        IntervalDictionary.Clear();
        _rubyList.Clear();
        string processingText = text;
        //.     代表任意字符
        //*     代表前一个字符出现0次或多次
        //+     代表前一个字符出现一次或多次
        //?     代表前一个字符出现0次或一次
        string pattern = "<.*?>";   //  .*？：匹配最短的结果
        Match match = Regex.Match(processingText, pattern);
        while (match.Success)
        {
            string label = match.Value.Substring(1, match.Length - 2);   //<>内的内容
            if (float.TryParse(label, out float result))
            {
                IntervalDictionary[match.Index - 1] = result; //匹配到的下标(<)-1
            }
            else if (Regex.IsMatch(label, "^r=.+"))   //匹配<r=”注解”></r>
            {
                //获得r= 后面的字符串
                _rubyList.Add(new RubyData(match.Index, label.Substring(2)));
            }
            else if (label == "/r")
            {
                if (_rubyList.Count > 0)
                {
                    _rubyList[_rubyList.Count - 1].EndIndex = match.Index - 1;
                }
            }
            //删掉<....>
            processingText = processingText.Remove(match.Index, match.Length);
            //<sorite=dsadsa>  的内容用*替代
            if (Regex.IsMatch(label, "^sprite =.+"))
            {
                processingText = processingText.Insert(match.Index,"*");         //占位，防止延迟调用的时候出错。
            }
            //重新匹配
            match = Regex.Match(processingText, pattern);
        }
        processingText = text;
        pattern = @"(<(\d+)(\.\d+)?>)|(</r>)|(<r=.*?>)";      //正则表达式：+=>前一个字符一次或多次
                                                                       //          ：()=>代表括号内的内容可以省略
        processingText = Regex.Replace(processingText, pattern,"");  //将符合pattern的替换成””
        return processingText;
    }
}
