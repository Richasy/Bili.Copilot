// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 下载视图模型.
/// </summary>
public sealed partial class DownloadViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadViewModel"/> class.
    /// </summary>
    public DownloadViewModel(
        ILogger<DownloadViewModel> logger,
        IBiliTokenResolver tokenResolver,
        IBiliCookiesResolver cookiesResolver)
    {
        _logger = logger;
        _tokenResolver = tokenResolver;
        _cookiesResolver = cookiesResolver;
        _bbdownPath = this.Get<AppViewModel>().BBDownPath;
        _ffmpegPath = this.Get<AppViewModel>().FFmpegPath;
        var localDownloadFolder = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DownloadFolder, string.Empty);
        _downloadPath = string.IsNullOrEmpty(localDownloadFolder) || !Directory.Exists(localDownloadFolder)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Bili Downloads")
            : localDownloadFolder;
        if (!Directory.Exists(_downloadPath))
        {
            Directory.CreateDirectory(_downloadPath);
        }

        _openFolderAfterDownload = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.OpenFolderAfterDownload, true);
    }

    /// <summary>
    /// 初始化基础数据.
    /// </summary>
    public void InitializeMetas(
        string url,
        IReadOnlyCollection<PlayerFormatInformation> formats,
        IReadOnlyCollection<VideoPart>? parts = default,
        int currentPart = 1)
    {
        _currentUrl = url;
        _currentPartIndex = currentPart;
        Formats = formats;
        Parts = parts;
        MetaInitialized?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 初始化基础数据.
    /// </summary>
    public void InitializeMetas(
        string epUrl,
        string ssUrl,
        IReadOnlyCollection<PlayerFormatInformation> formats,
        IReadOnlyCollection<EpisodeInformation>? episodes = default,
        int currentEpisode = 1)
    {
        _currentUrl = epUrl;
        _parentUrl = ssUrl;
        _currentPartIndex = currentEpisode;
        Formats = formats;
        Episodes = episodes;
        MetaInitialized?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void Clear()
    {
        _currentUrl = string.Empty;
        _parentUrl = string.Empty;
        _currentPartIndex = 0;
        Formats = null;
        Parts = null;
        Episodes = null;
    }

    /// <summary>
    /// 下载视频.
    /// </summary>
    [RelayCommand]
    private void DownloadWithFormat(PlayerFormatInformation? format = default)
    {
        var basicCommand = GetBasicCommand();
        if (_currentPartIndex > 1 && !_currentUrl.Contains("/ep"))
        {
            basicCommand += $" --select-page {_currentPartIndex}";
        }

        var command = $"{basicCommand} -q \"{format.Quality}\" \"{_currentUrl}\"";
        LaunchDownloadProcess(command);
    }

    [RelayCommand]
    private void DownloadCover()
    {
        var command = $"{GetBasicCommand(false)} --cover-only \"{_currentUrl}\"";
        LaunchDownloadProcess(command);
    }

    [RelayCommand]
    private void DownloadDanmaku()
    {
        var command = $"{GetBasicCommand(false)} --danmaku-only \"{_currentUrl}\"";
        LaunchDownloadProcess(command);
    }

    [RelayCommand]
    private void BatchDownloadAllParts()
    {
        var url = _currentUrl.Contains("/ep") ? _parentUrl : _currentUrl;
        var basicCommand = GetBasicCommand();
        basicCommand += " --select-page ALL";
        var command = $"{basicCommand} \"{url}\"";
        LaunchDownloadProcess(command);
    }

    [RelayCommand]
    private void BatchDownloadSelectedParts(string selection)
    {
        var url = _currentUrl.Contains("/ep") ? _parentUrl : _currentUrl;
        var basicCommand = GetBasicCommand();
        basicCommand += $" --select-page {selection}";
        var command = $"{basicCommand} \"{url}\"";
        LaunchDownloadProcess(command);
    }

    private string GetBasicCommand(bool danmakuEnabled = true)
    {
        var token = _tokenResolver.GetToken().AccessToken;
        var cookie = _cookiesResolver.GetCookieString();
        var preferCodec = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PreferCodec, PreferCodecType.H264);
        var codec = preferCodec switch
        {
            PreferCodecType.H264 => "avc",
            PreferCodecType.H265 => "hevc",
            _ => "av1",
        };
        var cmd = $"-app --cookie \"{cookie}\" -token \"{token}\" -e {codec} --work-dir \"{_downloadPath}\" --ffmpeg-path \"{_ffmpegPath}\" -ua \"{VideoUserAgent}\"";
        if (danmakuEnabled)
        {
            var alsoDownDanmaku = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DownloadWithDanmaku, false);
            if (alsoDownDanmaku)
            {
                cmd += " -dd";
            }
        }

        return cmd;
    }

    private void LaunchDownloadProcess(string command)
    {
        _ = Task.Run(() =>
        {
            var startInfo = new ProcessStartInfo(_bbdownPath, command);
            var process = Process.Start(startInfo);
            if (_openFolderAfterDownload)
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    Process.Start("explorer.exe", _downloadPath);
                }
            }
        });
    }
}
