// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.Libs.Toolkit;

/// <summary>
/// 文本工具.
/// </summary>
public static class TextToolkit
{
    /// <summary>
    /// 如果当前语言环境为繁体中文，那就将传入文本转换为繁体中文.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <returns>繁体文本或普通文本.</returns>
    public static string ConvertToTraditionalChineseIfNeeded(string text)
    {
        var lan = SettingsToolkit.ReadLocalSetting(SettingNames.LastAppLanguage, "zh-Hans-CN");
        var needConvert = SettingsToolkit.ReadLocalSetting(SettingNames.IsFullTraditionalChinese, false);

        if (!lan.Contains("zh-hant", System.StringComparison.OrdinalIgnoreCase)
            || !needConvert
            || string.IsNullOrEmpty(text))
        {
            return text;
        }

        var type = 0;
        if (lan.Contains("hk", System.StringComparison.OrdinalIgnoreCase))
        {
            // 香港地区.
            type = 1;
        }
        else if (lan.Contains("tw", System.StringComparison.OrdinalIgnoreCase))
        {
            // 台湾地区.
            type = 2;
        }

        return ToolGood.Words.WordsHelper.ToTraditionalChinese(text, type);
    }
}
