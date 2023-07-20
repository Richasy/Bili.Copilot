// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bili.Copilot.Libs.Player.Core;

/// <summary>
/// 表示一种语言.
/// </summary>
public class Language : IEquatable<Language>
{
    private string _cultureName;

    /// <summary>
    /// 表示英语语言.
    /// </summary>
    public static Language English { get; } = Get("eng");

    /// <summary>
    /// 表示中文语言.
    /// </summary>
    public static Language Chinese { get; } = Get("zho");

    /// <summary>
    /// 表示未知语言.
    /// </summary>
    public static Language Unknown { get; } = Get("und");

    /// <summary>
    /// 从 ISO639-2T 代码到 ISO639-2B 代码的映射表.
    /// </summary>
    public static Dictionary<string, string> ISO639_2T_TO_2B { get; } = new()
    {
        { "bod", "tib" },
        { "ces", "cze" },
        { "cym", "wel" },
        { "deu", "ger" },
        { "ell", "gre" },
        { "eus", "baq" },
        { "fas", "per" },
        { "fra", "fre" },
        { "hye", "arm" },
        { "isl", "ice" },
        { "kat", "geo" },
        { "mkd", "mac" },
        { "mri", "mao" },
        { "msa", "may" },
        { "mya", "bur" },
        { "nld", "dut" },
        { "ron", "rum" },
        { "slk", "slo" },
        { "sqi", "alb" },
        { "zho", "chi" },
    };

    /// <summary>
    /// 从 ISO639-2B 代码到 ISO639-2T 代码的映射表.
    /// </summary>
    public static Dictionary<string, string> ISO639_2B_TO_2T { get; } = new()
    {
        { "alb", "sqi" },
        { "arm", "hye" },
        { "baq", "eus" },
        { "bur", "mya" },
        { "chi", "zho" },
        { "cze", "ces" },
        { "dut", "nld" },
        { "fre", "fra" },
        { "geo", "kat" },
        { "ger", "deu" },
        { "gre", "ell" },
        { "ice", "isl" },
        { "mac", "mkd" },
        { "mao", "mri" },
        { "may", "msa" },
        { "per", "fas" },
        { "rum", "ron" },
        { "slo", "slk" },
        { "tib", "bod" },
        { "wel", "cym" },
    };

    /// <summary>
    /// 获取或设置语言的区域名称.
    /// </summary>
    public string CultureName
    {
        get => _cultureName;
        set
        {
            // 用于 XML 加载
            Culture = CultureInfo.GetCultureInfo(value);
            Refresh(this);
        }
    }

    /// <summary>
    /// 获取语言的顶级英文名称.
    /// </summary>
    public string TopEnglishName { get; private set; }

    /// <summary>
    /// 获取语言的区域信息.
    /// </summary>
    public CultureInfo Culture { get; private set; }

    /// <summary>
    /// 获取语言的顶级区域信息.
    /// </summary>
    public CultureInfo TopCulture { get; private set; }

    /// <summary>
    /// 获取语言的子语言 ID，可用于在线字幕搜索.
    /// </summary>
    public string IdSubLanguage { get; private set; }

    /// <summary>
    /// 获取原始输入的字符串，仅适用于未确定的语言（返回克隆）.
    /// </summary>
    public string OriginalInput { get; private set; }

    /// <summary>
    /// 判断两个语言是否相等.
    /// </summary>
    public static bool operator ==(Language lang1, Language lang2) => lang1 is null ? lang2 is null : lang1.Equals(lang2);

    /// <summary>
    /// 判断两个语言是否不相等.
    /// </summary>
    public static bool operator !=(Language lang1, Language lang2) => !(lang1 == lang2);

    /// <summary>
    /// 刷新语言的属性.
    /// </summary>
    public static void Refresh(Language lang)
    {
        lang._cultureName = lang.Culture.Name;

        lang.TopCulture = lang.Culture;
        while (lang.TopCulture.Parent.Name != string.Empty)
        {
            lang.TopCulture = lang.TopCulture.Parent;
        }

        lang.TopEnglishName = lang.TopCulture.EnglishName;
        lang.IdSubLanguage = lang.Culture.ThreeLetterISOLanguageName;
    }

    /// <summary>
    /// 根据区域信息获取语言.
    /// </summary>
    /// <returns><see cref="Language"/>.</returns>
    public static Language Get(CultureInfo cult)
    {
        Language lang = new() { Culture = cult };
        Refresh(lang);

        return lang;
    }

    /// <summary>
    /// 根据名称获取语言.
    /// </summary>
    /// <returns><see cref="Language"/>.</returns>
    public static Language Get(string name)
    {
        Language lang = new() { Culture = StringToCulture(name) };
        if (lang.Culture != null)
        {
            Refresh(lang);
        }
        else
        {
            lang.IdSubLanguage = "und";
            lang.TopEnglishName = "Unknown";
            if (name != "und")
            {
                lang.OriginalInput = name;
            }
        }

        return lang;
    }

    /// <summary>
    /// 将字符串转换为区域信息.
    /// </summary>
    /// <returns><see cref="CultureInfo"/>.</returns>
    public static CultureInfo StringToCulture(string lang)
    {
        if (string.IsNullOrWhiteSpace(lang) || lang.Length < 2)
        {
            return null;
        }

        var langLower = lang.ToLower();
        CultureInfo ret = null;

        try
        {
            ret = lang.Length == 3 ? ThreeLetterToCulture(langLower) : CultureInfo.GetCultureInfo(langLower);
        }
        catch
        {
        }

        // TBR: Check also -Country/region two letters?
        if (ret == null || ret.ThreeLetterISOLanguageName == string.Empty)
        {
            foreach (var cult in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (cult.Name.ToLower() == langLower || cult.NativeName.ToLower() == langLower || cult.EnglishName.ToLower() == langLower)
                {
                    return cult;
                }
            }
        }

        return ret;
    }

    /// <summary>
    /// 将三字母代码转换为区域信息.
    /// </summary>
    /// <returns><see cref="CultureInfo"/>.</returns>
    public static CultureInfo ThreeLetterToCulture(string lang)
    {
        if (lang == "zho")
        {
            return CultureInfo.GetCultureInfo("zh-CN");
        }
        else if (lang == "pob")
        {
            return CultureInfo.GetCultureInfo("pt-BR");
        }
        else if (lang == "nor")
        {
            return CultureInfo.GetCultureInfo("nob");
        }
        else if (lang == "scc")
        {
            return CultureInfo.GetCultureInfo("srp");
        }
        else if (lang == "tgl")
        {
            return CultureInfo.GetCultureInfo("fil");
        }

        var ret = CultureInfo.GetCultureInfo(lang);

        if (ret.ThreeLetterISOLanguageName == string.Empty)
        {
            ISO639_2B_TO_2T.TryGetValue(lang, out var iso639_2t);
            if (iso639_2t != null)
            {
                ret = CultureInfo.GetCultureInfo(iso639_2t);
            }
        }

        return ret.ThreeLetterISOLanguageName == string.Empty ? null : ret;
    }

    /// <inheritdoc/>
    public override string ToString() => OriginalInput ?? TopEnglishName;

    /// <inheritdoc/>
    public override int GetHashCode() => ToString().GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as Language);

    /// <inheritdoc/>
    public bool Equals(Language lang)
    {
        if (lang is null)
        {
            return false;
        }

        if (lang.Culture == null && Culture == null)
        {
            if (OriginalInput != null || lang.OriginalInput != null)
            {
                return OriginalInput == lang.OriginalInput;
            }

            return true; // und
        }

        return lang.IdSubLanguage == IdSubLanguage; // TBR: top level will be equal with lower
    }
}
