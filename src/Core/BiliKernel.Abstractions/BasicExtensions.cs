// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using System;

namespace Richasy.BiliKernel;

/// <summary>
/// 基础扩展.
/// </summary>
public static class BasicExtensions
{
    /// <summary>
    /// 将数字简写文本中转换为大略的次数.
    /// </summary>
    /// <param name="text">数字简写文本.</param>
    /// <param name="removeText">需要先在简写文本中移除的内容.</param>
    /// <returns>一个大概的次数，比如 <c>3.2万播放</c>，最终会返回 <c>32000</c>.</returns>
    public static double ToCountNumber(this string text, string removeText = "")
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

    public static int ToDurationSeconds(this string durationText)
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
            durationText = string.Join(":", "00", sp[1], sp[2]);
            hourStr = sp[0];
        }

        var ts = TimeSpan.Parse(durationText);
        if (!string.IsNullOrEmpty(hourStr))
        {
            ts += TimeSpan.FromHours(Convert.ToInt32(hourStr));
        }

        return Convert.ToInt32(ts.TotalSeconds);
    }
}
