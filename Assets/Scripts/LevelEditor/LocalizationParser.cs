using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class LocalizationParser
{
    public static string ParseText(string rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return string.Empty;

        // 1. 尝试匹配当前设置的语言标签
        string currentLangStr = Localization.GetLanguage().ToString();
        string currentPattern = "\\[" + currentLangStr + "\\](.*?)(?=\\[[a-zA-Z]+\\]|$)";
        Match match = Regex.Match(rawText, currentPattern, RegexOptions.Singleline);
        if (match.Success) return "\"" + match.Groups[1].Value.Trim() + "\"";

        // 2. 降级方案：尝试匹配英语标签
        string englishPattern = "\\[English\\](.*?)(?=\\[[a-zA-Z]+\\]|$)";
        match = Regex.Match(rawText, englishPattern, RegexOptions.Singleline);
        if (match.Success) return "\"" + match.Groups[1].Value.Trim() + "\"";

        // 3. 终极保底方案：匹配第一个出现的任意语言标签
        string anyPattern = "\\[[a-zA-Z]+\\](.*?)(?=\\[[a-zA-Z]+\\]|$)";
        match = Regex.Match(rawText, anyPattern, RegexOptions.Singleline);
        if (match.Success) return "\"" + match.Groups[1].Value.Trim() + "\"";

        // 如果没有任何标签，直接返回原文本
        return rawText;
    }
}