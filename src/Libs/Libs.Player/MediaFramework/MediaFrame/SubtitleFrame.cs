// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Bili.Copilot.Libs.Player.Enums;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;

/// <summary>
/// 字幕帧类，继承自帧基类.
/// </summary>
public class SubtitleFrame : FrameBase
{
    /// <summary>
    /// 构造函数，用于初始化字幕帧对象.
    /// </summary>
    /// <param name="text">字幕文本内容.</param>
    public SubtitleFrame(string text)
        => Text = text;

    /// <summary>
    /// 获取或设置字幕帧的持续时间.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 获取或设置字幕帧的文本内容.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 获取或设置字幕帧的字幕样式列表.
    /// </summary>
    public List<SubtitleStyle> SubtitleStyles { get; set; }
}

/// <summary>
/// 字幕样式结构体.
/// </summary>
public struct SubtitleStyle
{
    /// <summary>
    /// 字幕样式.
    /// </summary>
    public SubtitleStyleType Style;

    /// <summary>
    /// 字幕颜色.
    /// </summary>
    public Color Value;

    /// <summary>
    /// 字幕起始位置.
    /// </summary>
    public int From;

    /// <summary>
    /// 字幕长度.
    /// </summary>
    public int Length;

    /// <summary>
    /// 构造函数，根据起始位置、长度和颜色创建字幕样式.
    /// </summary>
    /// <param name="from">起始位置.</param>
    /// <param name="len">长度.</param>
    /// <param name="value">颜色.</param>
    public SubtitleStyle(int from, int len, Color value)
        : this(SubtitleStyleType.Color, from, len, value)
    {
    }

    /// <summary>
    /// 构造函数，根据样式、起始位置、长度和颜色创建字幕样式.
    /// </summary>
    /// <param name="style">样式.</param>
    /// <param name="from">起始位置.</param>
    /// <param name="len">长度.</param>
    /// <param name="value">颜色.</param>
    public SubtitleStyle(SubtitleStyleType style, int from = -1, int len = -1, Color? value = null)
    {
        Style = style;
        Value = value == null ? Color.White : (Color)value;
        From = from;
        Length = len;
    }
}

/// <summary>
/// 默认的字幕解析器.
/// </summary>
public static class ParseSubtitle
{
    /// <summary>
    /// 解析字幕帧.
    /// </summary>
    /// <param name="sFrame">要解析的字幕帧.</param>
    public static void Parse(SubtitleFrame sFrame)
    {
        sFrame.Text = SSAtoSubtitleStyleType(sFrame.Text, out var subStyles);
        sFrame.SubtitleStyles = subStyles;
    }

    /// <summary>
    /// 将 SSA 字符串转换为字幕样式类型.
    /// </summary>
    /// <param name="s">要转换的 SSA 字符串.</param>
    /// <param name="styles">转换后的字幕样式列表.</param>
    /// <returns>转换后的字符串.</returns>
    public static string SSAtoSubtitleStyleType(string s, out List<SubtitleStyle> styles)
    {
        var pos = 0;
        var sout = string.Empty;
        styles = new List<SubtitleStyle>();

        SubtitleStyle bold = new(SubtitleStyleType.Bold);
        SubtitleStyle italic = new(SubtitleStyleType.Italic);
        SubtitleStyle underline = new(SubtitleStyleType.Underline);
        SubtitleStyle strikeout = new(SubtitleStyleType.StrikeOut);
        SubtitleStyle color = new(SubtitleStyleType.Color);

        s = s.LastIndexOf(",,") == -1 ? s : s[(s.LastIndexOf(",,") + 2) ..].Replace("\\N", "\n").Trim();

        for (var i = 0; i < s.Length; i++)
        {
            if (s[i] == '{')
            {
                continue;
            }

            if (s[i] == '\\' && s[i - 1] == '{')
            {
                var codeLen = s.IndexOf('}', i) - i;
                if (codeLen == -1)
                {
                    continue;
                }

                var code = s.Substring(i, codeLen).Trim();

                switch (code[1])
                {
                    case 'c':
                        if (code.Length == 2)
                        {
                            if (color.From == -1)
                            {
                                break;
                            }

                            color.Length = pos - color.From;
                            if (color.Value != Color.Transparent)
                            {
                                styles.Add(color);
                            }

                            color = new SubtitleStyle(SubtitleStyleType.Color);
                        }
                        else
                        {
                            color.From = pos;
                            color.Value = Color.Transparent;
                            if (code.Length < 7)
                            {
                                break;
                            }

                            var colorEnd = code.LastIndexOf("&");
                            if (colorEnd < 6)
                            {
                                break;
                            }

                            var hexColor = code[4..colorEnd];
                            var red = int.Parse(hexColor.Substring(hexColor.Length - 2, 2), NumberStyles.HexNumber);
                            var green = 0;
                            var blue = 0;

                            if (hexColor.Length - 2 > 0)
                            {
                                hexColor = hexColor[..^2];
                                green = int.Parse(hexColor.Substring(hexColor.Length - 2, 2), NumberStyles.HexNumber);
                            }

                            if (hexColor.Length - 2 > 0)
                            {
                                hexColor = hexColor[..^2];
                                blue = int.Parse(hexColor.Substring(hexColor.Length - 2, 2), NumberStyles.HexNumber);
                            }

                            color.Value = Color.FromArgb(255, red, green, blue);
                        }

                        break;

                    case 'b':
                        if (code[2] == '0')
                        {
                            if (bold.From == -1)
                            {
                                break;
                            }

                            bold.Length = pos - bold.From;
                            styles.Add(bold);
                            bold = new SubtitleStyle(SubtitleStyleType.Bold);
                        }
                        else
                        {
                            bold.From = pos;
                        }

                        break;

                    case 'u':
                        if (code[2] == '0')
                        {
                            if (underline.From == -1)
                            {
                                break;
                            }

                            underline.Length = pos - underline.From;
                            styles.Add(underline);
                            underline = new SubtitleStyle(SubtitleStyleType.Underline);
                        }
                        else
                        {
                            underline.From = pos;
                        }

                        break;

                    case 's':
                        if (code[2] == '0')
                        {
                            if (strikeout.From == -1)
                            {
                                break;
                            }

                            strikeout.Length = pos - strikeout.From;
                            styles.Add(strikeout);
                            strikeout = new SubtitleStyle(SubtitleStyleType.StrikeOut);
                        }
                        else
                        {
                            strikeout.From = pos;
                        }

                        break;

                    case 'i':
                        if (code[2] == '0')
                        {
                            if (italic.From == -1)
                            {
                                break;
                            }

                            italic.Length = pos - italic.From;
                            styles.Add(italic);
                            italic = new SubtitleStyle(SubtitleStyleType.Italic);
                        }
                        else
                        {
                            italic.From = pos;
                        }

                        break;
                }

                i += codeLen;
                continue;
            }

            sout += s[i];
            pos++;
        }

        var soutPostLast = sout.Length;
        if (bold.From != -1)
        {
            bold.Length = soutPostLast - bold.From;
            styles.Add(bold);
        }

        if (italic.From != -1)
        {
            italic.Length = soutPostLast - italic.From;
            styles.Add(italic);
        }

        if (strikeout.From != -1)
        {
            strikeout.Length = soutPostLast - strikeout.From;
            styles.Add(strikeout);
        }

        if (underline.From != -1)
        {
            underline.Length = soutPostLast - underline.From;
            styles.Add(underline);
        }

        if (color.From != -1 && color.Value != Color.Transparent)
        {
            color.Length = soutPostLast - color.From;
            styles.Add(color);
        }

        return sout;
    }
}
