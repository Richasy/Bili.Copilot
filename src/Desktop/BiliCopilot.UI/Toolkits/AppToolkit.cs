﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Richasy.WinUIKernel.Share.Toolkits;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 应用工具组.
/// </summary>
internal sealed partial class AppToolkit : IAppToolkit
{
    /// <summary>
    /// 获取应用包版本.
    /// </summary>
    /// <returns>包版本.</returns>
    public string GetPackageVersion()
    {
        var appVersion = Package.Current.Id.Version;
        return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
    }

    /// <summary>
    /// 格式化时长.
    /// </summary>
    /// <returns>时长文本，如 01:33.</returns>
    public static string FormatDuration(TimeSpan duration)
        => duration.TotalHours >= 1 ? duration.ToString(@"hh\:mm\:ss") : duration.ToString(@"mm\:ss");

    /// <summary>
    /// 格式化数量.
    /// </summary>
    /// <returns>数量文本，如 233万.</returns>
    public static string FormatCount(long count)
    {
        if (count < 0)
        {
            return string.Empty;
        }

        if (count >= 100_000_000)
        {
            var unit = ResourceToolkit.GetLocalizedString(StringNames.Billion);
            return Math.Round(count / 100_000_000d, 2) + unit;
        }
        else if (count >= 10_000)
        {
            var unit = ResourceToolkit.GetLocalizedString(StringNames.TenThousands);
            return Math.Round(count / 10_000d, 2) + unit;
        }

        return count.ToString();
    }

    /// <summary>
    /// 将颜色代码转换为Windows.UI.Color对象.
    /// </summary>
    /// <param name="hexCode">颜色代码.</param>
    /// <returns><see cref="Windows.UI.Color"/>.</returns>
    public static Windows.UI.Color HexToColor(string hexCode)
    {
        // 去除可能包含的 # 符号
        if (hexCode.StartsWith("#"))
        {
            hexCode = hexCode[1..];
        }

        if (int.TryParse(hexCode, out var intValue))
        {
            hexCode = intValue.ToString("X2");
        }

        var color = default(Windows.UI.Color);
        if (hexCode.Length == 4)
        {
            hexCode = "00" + hexCode;
        }

        if (hexCode.Length == 6)
        {
            color.R = byte.Parse(hexCode[..2], NumberStyles.HexNumber);
            color.G = byte.Parse(hexCode.Substring(2, 2), NumberStyles.HexNumber);
            color.B = byte.Parse(hexCode.Substring(4, 2), NumberStyles.HexNumber);
            color.A = 255;
        }

        if (hexCode.Length == 8)
        {
            color.R = byte.Parse(hexCode.Substring(2, 2), NumberStyles.HexNumber);
            color.G = byte.Parse(hexCode.Substring(4, 2), NumberStyles.HexNumber);
            color.B = byte.Parse(hexCode.Substring(6, 2), NumberStyles.HexNumber);
            color.A = byte.Parse(hexCode[..2], NumberStyles.HexNumber);
        }

        return color;
    }

    /// <summary>
    /// 获取偏好解码模式.
    /// </summary>
    /// <returns>解码标识符.</returns>
    public static string GetPreferCodecId()
    {
        var preferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodecType.H264);
        return preferCodec switch
        {
            PreferCodecType.H265 => "hev",
            PreferCodecType.Av1 => "av01",
            _ => "avc",
        };
    }

    /// <summary>
    /// 是否为P2P地址.
    /// </summary>
    /// <returns>检查结果.</returns>
    public static bool IsP2PUrl(string url)
    {
        var uri = new Uri(url);
        return P2PRegex().IsMatch(uri.Host);
    }

    /// <summary>
    /// 重置控件主题.
    /// </summary>
    /// <param name="element">控件.</param>
    public void ResetControlTheme(FrameworkElement element)
        => element.RequestedTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);

    /// <summary>
    /// 获取当前主题.
    /// </summary>
    /// <returns><see cref="ApplicationTheme"/>.</returns>
    public ApplicationTheme GetCurrentTheme()
    {
        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        return localTheme == ElementTheme.Default ? App.Current.RequestedTheme : localTheme == ElementTheme.Light ? ApplicationTheme.Light : ApplicationTheme.Dark;
    }

    /// <summary>
    /// 获取WebDav服务器地址.
    /// </summary>
    /// <returns>地址.</returns>
    public static string GetWebDavServer(string server, string path)
        => $"{server.TrimEnd('/')}/{path.TrimStart('/')}";

    [GeneratedRegex(@"(mcdn.bilivideo.(cn|com)|szbdyd.com)")]
    private static partial Regex P2PRegex();
}
