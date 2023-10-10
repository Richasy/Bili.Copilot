// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Microsoft.AppCenter.Analytics;
using Windows.Globalization;

namespace Bili.Copilot.App;

/// <summary>
/// 该类用户记录用户行为，并上传至 AppCenter 进行数据分析.
/// </summary>
/// <remarks>
/// 涉及用户隐私的数据不应该上传至 AppCenter，例如用户的昵称、头像等.
/// </remarks>
internal sealed class TraceLogger
{
    private const string AppLaunchedEvent = "AppLaunched";
    private const string UnhandledExceptionEvent = "UnhandledException";
    private const string CloseWindowTipEvent = "CloseWindowTip";
    private const string SignOutEvent = "SignOut";
    private const string SignInEvent = "SignIn";
    private const string HotSearchClickEvent = "HotSearchClick";
    private const string MainPageNavigationEvent = "MainPageNavigation";
    private const string PlayerOpenEvent = "PlayerOpen";
    private const string OnlyAudioChangedEvent = "OnlyAudioChanged";
    private const string RecordingStartEvent = "RecordingStart";
    private const string TakeScreenshotEvent = "TakeScreenshot";
    private const string PlayerOpenInBrowserEvent = "PlayerOpenInBrowser";
    private const string PlayInPrivateEvent = "PlayInPrivate";
    private const string AIOpenEvent = "AIOpen";
    private const string DownloadEvent = "Download";
    private const string DanmakuStatusChangedEvent = "DanmakuStatusChanged";
    private const string PlayerDisplayModeChangedEvent = "PlayerDisplayModeChanged";

    private readonly string _version;
    private readonly string _language;
    private readonly string _systemVersion;

    private TraceLogger()
    {
        _version = AppToolkit.GetPackageVersion();
        _language = ApplicationLanguages.Languages.FirstOrDefault() ?? "Unknown";
        _systemVersion = Environment.OSVersion.VersionString;
    }

    public static TraceLogger Instance { get; } = new();

    public static void LogCloseWindowTip(bool neverAsk, string type)
    {
        var data = new Dictionary<string, string>
        {
            { "NeverAsk", neverAsk.ToString() },
            { "Type", type },
        };

        Analytics.TrackEvent(CloseWindowTipEvent, data);
    }

    public static void LogSignIn()
        => Analytics.TrackEvent(SignInEvent);

    public static void LogSignOut()
        => Analytics.TrackEvent(SignOutEvent);

    public static void LogHotSearchClick()
        => Analytics.TrackEvent(HotSearchClickEvent);

    public static void LogMainPageNavigation(string pageName)
    {
        var data = new Dictionary<string, string>
        {
            { "PageName", pageName },
        };

        Analytics.TrackEvent(MainPageNavigationEvent, data);
    }

    public static void LogPlayerOpen(
        string videoType,
        string preferCodec,
        string preferQuality,
        string decodeType,
        string playerType)
    {
        var data = new Dictionary<string, string>
        {
            { "VideoType", videoType },
            { "PreferCodec", preferCodec },
            { "PreferQuality", preferQuality },
            { "DecodeType", decodeType },
            { "PlayerType", playerType },
        };

        Analytics.TrackEvent(PlayerOpenEvent, data);
    }

    /// <summary>
    /// 查看有多少用户使用纯音频功能.
    /// </summary>
    /// <param name="isOn">是否开启.</param>
    /// <param name="videoType">视频类型（以此判断该功能的适用场景）.</param>
    public static void LogOnlyAudioChanged(bool isOn, string videoType)
    {
        var data = new Dictionary<string, string>
        {
            { "IsOn", isOn.ToString() },
            { "VideoType", videoType },
        };

        Analytics.TrackEvent(OnlyAudioChangedEvent, data);
    }

    /// <summary>
    /// 查看有多少用户使用录制功能.
    /// </summary>
    public static void LogRecordingStart()
        => Analytics.TrackEvent(RecordingStartEvent);

    /// <summary>
    /// 查看有多少用户使用截图功能.
    /// </summary>
    public static void LogTakeScreenshot()
        => Analytics.TrackEvent(TakeScreenshotEvent);

    /// <summary>
    /// 查看有多少用户使用下载功能.
    /// </summary>
    public static void LogDownload(string videoType)
    {
        var data = new Dictionary<string, string>
        {
            { "VideoType", videoType },
        };

        Analytics.TrackEvent(DownloadEvent, data);
    }

    /// <summary>
    /// 这意味着视频解析失败，据此判断打开视频失败的比例.
    /// </summary>
    public static void LogPlayerOpenInBrowser()
        => Analytics.TrackEvent(PlayerOpenInBrowserEvent);

    /// <summary>
    /// 查看有多少用户倾向于使用隐私模式.
    /// </summary>
    public static void LogPlayInPrivate()
        => Analytics.TrackEvent(PlayInPrivateEvent);

    public static void LogDanmakuStatusChanged(bool isOn)
    {
        var data = new Dictionary<string, string>
        {
            { "IsOn", isOn.ToString() },
        };

        Analytics.TrackEvent(DanmakuStatusChangedEvent, data);
    }

    /// <summary>
    /// 查看有多少用户使用 AI 功能.
    /// </summary>
    /// <param name="featureType">功能类型.</param>
    /// <param name="connectType">与小幻助理的连接类型.</param>
    public static void LogAIOpen(string featureType, string connectType)
    {
        var data = new Dictionary<string, string>
        {
            { "FeatureType", featureType },
            { "ConnectType", connectType },
        };

        Analytics.TrackEvent(AIOpenEvent, data);
    }

    public static void LogPlayerDisplayModeChanged(string displayMode)
    {
        var data = new Dictionary<string, string>
        {
            { "DisplayMode", displayMode },
        };

        Analytics.TrackEvent(PlayerDisplayModeChangedEvent, data);
    }

    public void LogAppLaunched()
    {
        var data = new Dictionary<string, string>
        {
            { "Language", _language },
            { "SystemVersion", _systemVersion },
            { "Version", _version },
        };
        Analytics.TrackEvent(AppLaunchedEvent, data);
    }

    public void LogUnhandledException(Exception ex)
    {
        var data = new Dictionary<string, string>
        {
            { "SystemVersion", _systemVersion },
            { "Version", _version },
            { "Exception", ex.ToString() },
        };

        Analytics.TrackEvent(UnhandledExceptionEvent, data);
    }
}
