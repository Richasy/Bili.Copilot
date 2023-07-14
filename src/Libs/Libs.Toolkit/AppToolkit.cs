// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Bili.Copilot.Models.Constants.App;
using Windows.ApplicationModel;

namespace Bili.Copilot.Libs.Toolkit;

/// <summary>
/// 应用工具组.
/// </summary>
public static class AppToolkit
{
    /// <summary>
    /// Get the current environment language code.
    /// </summary>
    /// <param name="isWindowsName">
    /// Whether it is the Windows display name,
    /// for example, Simplified Chinese is CHS,
    /// if not, it is displayed as the default name,
    /// for example, Simplified Chinese is zh-Hans.
    /// </param>
    /// <returns>Language code.</returns>
    public static string GetLanguageCode(bool isWindowsName = false)
    {
        var culture = CultureInfo.CurrentUICulture;
        return isWindowsName ? culture.ThreeLetterWindowsLanguageName : culture.Name;
    }

    /// <summary>
    /// 获取应用包版本.
    /// </summary>
    /// <returns>包版本.</returns>
    public static string GetPackageVersion()
    {
        var appVersion = Package.Current.Id.Version;
        return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
    }

    /// <summary>
    /// 获取应用设置的代理以及对应内容的区域.
    /// </summary>
    /// <param name="title">视频标题.</param>
    /// <param name="isVideo">是否为 UGC 视频.</param>
    /// <returns>代理及区域.</returns>
    public static Tuple<string, string> GetProxyAndArea(string title, bool isVideo)
    {
        var proxy = string.Empty;
        var area = string.Empty;

        var isOpenRoaming = SettingsToolkit.ReadLocalSetting(SettingNames.IsOpenRoaming, false);
        var localProxy = isVideo
            ? SettingsToolkit.ReadLocalSetting(SettingNames.RoamingVideoAddress, string.Empty)
            : SettingsToolkit.ReadLocalSetting(SettingNames.RoamingViewAddress, string.Empty);
        if (isOpenRoaming && !string.IsNullOrEmpty(localProxy))
        {
            if (!string.IsNullOrEmpty(title))
            {
                if (Regex.IsMatch(title, @"僅.*港.*地區"))
                {
                    area = "hk";
                }
                else if (Regex.IsMatch(title, @"僅.*台.*地區"))
                {
                    area = "tw";
                }
            }

            var isForceProxy = SettingsToolkit.ReadLocalSetting(SettingNames.IsGlobeProxy, false);
            if ((isForceProxy && string.IsNullOrEmpty(area))
                || !string.IsNullOrEmpty(area))
            {
                proxy = localProxy;
            }
        }

        return new Tuple<string, string>(proxy, area);
    }
}
