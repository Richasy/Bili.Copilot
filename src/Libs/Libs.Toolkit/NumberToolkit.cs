// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.Libs.Toolkit;

/// <summary>
/// 数字处理工具.
/// </summary>
public static class NumberToolkit
{
    /// <summary>
    /// 获取次数的中文简写文本.
    /// </summary>
    /// <param name="count">次数.</param>
    /// <returns>简写文本.</returns>
    public static string GetCountText(double count)
    {
        if (count < 0)
        {
            return string.Empty;
        }

        if (count >= 100000000)
        {
            var unit = ResourceToolkit.GetLocalizedString(StringNames.Billion);
            return Math.Round(count / 100000000, 2) + unit;
        }
        else if (count >= 10000)
        {
            var unit = ResourceToolkit.GetLocalizedString(StringNames.TenThousands);
            return Math.Round(count / 10000, 2) + unit;
        }

        return count.ToString();
    }

    /// <summary>
    /// 将数字简写文本中转换为大略的次数.
    /// </summary>
    /// <param name="text">数字简写文本.</param>
    /// <param name="removeText">需要先在简写文本中移除的内容.</param>
    /// <returns>一个大概的次数，比如 <c>3.2万播放</c>，最终会返回 <c>32000</c>.</returns>
    public static double GetCountNumber(string text, string removeText = "")
    {
        if (!string.IsNullOrEmpty(removeText))
        {
            text = text.Replace(removeText, string.Empty).Trim();
        }

        // 对于目前的B站来说，汉字单位只有 `万` 和 `亿` 两种.
        if (text.EndsWith("万"))
        {
            var num = Convert.ToDouble(text.Replace("万", string.Empty));
            return num * 10000;
        }
        else if (text.EndsWith("亿"))
        {
            var num = Convert.ToDouble(text.Replace("亿", string.Empty));
            return num * 100000000;
        }

        return double.TryParse(text, out var number) ? number : -1;
    }

    /// <summary>
    /// 获取时长的文本描述.
    /// </summary>
    /// <param name="timeSpan">时长.</param>
    /// <returns>文本描述.</returns>
    public static string GetDurationText(TimeSpan timeSpan)
    {
        return timeSpan.TotalHours > 1
            ? Math.Round(timeSpan.TotalHours, 2) + " " + ResourceToolkit.GetLocalizedString(StringNames.Hours)
            : timeSpan.TotalMinutes > 1
                ? Math.Round(timeSpan.TotalMinutes) + " " + ResourceToolkit.GetLocalizedString(StringNames.Minutes)
                : Math.Round(timeSpan.TotalSeconds) + " " + ResourceToolkit.GetLocalizedString(StringNames.Seconds);
    }

    /// <summary>
    /// 对网络数据的时长字符串进行格式调整，网络数据格式为：hh:mm:ss.
    /// </summary>
    /// <param name="webDurationText">网络时长数据.</param>
    /// <returns>重新解析后的可读时长文本.</returns>
    public static string FormatDurationText(string webDurationText)
    {
        var sec = GetDurationSeconds(webDurationText);
        return GetDurationText(TimeSpan.FromSeconds(sec));
    }

    /// <summary>
    /// 将时长文本转化为秒数.
    /// </summary>
    /// <param name="durationText">时长文本，比如 0:44，表示 44 秒.</param>
    /// <returns>对应的秒数.</returns>
    public static int GetDurationSeconds(string durationText)
    {
        var colonCount = durationText.Count(p => p == ':');
        var hourStr = string.Empty;
        if (colonCount == 1)
        {
            durationText = "00:" + durationText;
        }
        else if (colonCount == 2)
        {
            var sp = durationText.Split(':');
            durationText = string.Join(':', "00", sp[1], sp[2]);
            hourStr = sp[0];
        }

        var ts = TimeSpan.Parse(durationText);
        if (!string.IsNullOrEmpty(hourStr))
        {
            ts += TimeSpan.FromHours(Convert.ToInt32(hourStr));
        }

        return Convert.ToInt32(ts.TotalSeconds);
    }

    /// <summary>
    /// 对时长数据进行格式化，转换为 hh:mm:ss 格式的文本.
    /// </summary>
    /// <param name="ts">时长数据.</param>
    /// <param name="hasHours">是否需要补全小时位.</param>
    /// <returns>重新解析后的可读时长文本.</returns>
    public static string FormatDurationText(TimeSpan ts, bool hasHours)
    {
        return hasHours
            ? $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}"
            : $"{Math.Floor(ts.TotalMinutes):00}:{ts.Seconds:00}";
    }
}
