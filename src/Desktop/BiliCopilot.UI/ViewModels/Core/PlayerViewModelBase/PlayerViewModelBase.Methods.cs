﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using Richasy.BiliKernel.Bili.Authorization;
using System.Diagnostics;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型基类.
/// </summary>
public abstract partial class PlayerViewModelBase
{
    /// <summary>
    /// 尝试加载播放数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected async Task TryLoadPlayDataAsync()
    {
        if ((string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl)) || _isClosed)
        {
            return;
        }

        IsPlayerDataLoading = true;
        IsPaused = true;

        await OnLoadPlayDataAsync();

        PlayerDataLoaded?.Invoke(this, EventArgs.Empty);
        IsPlayerDataLoading = false;
    }

    /// <summary>
    /// 设置 WebDAV 配置.
    /// </summary>
    protected abstract void SetWebDavConfig(WebDavConfig config);

    private static string GetScreenshotFolderPath()
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bili-Screenshots");

    private void ActiveDisplay()
    {
        if (_displayRequest != null)
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest = new Windows.System.Display.DisplayRequest();
            _displayRequest.RequestActive();
        });
    }

    private void ReleaseDisplay()
    {
        if (_displayRequest == null)
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest?.RequestRelease();
            _displayRequest = null;
        });
    }

    private void OpenWithMpvOrMpvNet(bool isMpv)
    {
        var httpParams =
            IsWebDav
            ? $"--http-header-fields=\\\"Authorization: Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavConfig.UserName}:{_webDavConfig.Password}"))}\\\""
            : IsLive
                ? $"--cookies --no-ytdl --http-header-fields=\\\"Cookie:{this.Get<IBiliCookiesResolver>().GetCookieString()}\\\" --http-header-fields=\\\"Referer:{LiveReferer}\\\" --user-agent \\\"{LiveUserAgent}\\\""
                : $"--cookies --http-header-fields=\\\"Cookie:{this.Get<IBiliCookiesResolver>().GetCookieString()}\\\" --http-header-fields=\\\"Referer:{VideoReferer}\\\" --user-agent=\\\"{VideoUserAgent}\\\"";
        var exeName = isMpv ? "mpv" : "mpvnet";
        if (!string.IsNullOrEmpty(_extraOptions))
        {
            httpParams += $" --script-opts=\\\"cid={_extraOptions}\\\"";
        }

        var command = $"{exeName} {httpParams} --title=\\\"{Title}\\\" \\\"{_videoUrl}\\\"";
        if (!string.IsNullOrEmpty(_audioUrl))
        {
            command += $" --audio-file=\\\"{_audioUrl}\\\"";
        }

        _ = Task.Run(() =>
        {
            var startInfo = new ProcessStartInfo("powershell.exe", $"-Command \"{command}\"");
            var process = Process.Start(startInfo);
        });
    }
}
