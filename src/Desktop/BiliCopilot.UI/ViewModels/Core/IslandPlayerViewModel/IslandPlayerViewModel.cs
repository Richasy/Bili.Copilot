// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 岛播放器视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class IslandPlayerViewModel : PlayerViewModelBase
{
    private HWND _playerWindowHandle;
    private bool _isLoaded;

    /// <summary>
    /// 设置窗口句柄.
    /// </summary>
    public void SetWindow(IntPtr windowHandle)
    {
        _playerWindowHandle = new(windowHandle);
        IsPlayerInitializing = true;
        IsPlayerInitializing = false;
        _isInitialized = true;
        _dispatcherQueue.TryEnqueue(async () =>
        {
            await TryLoadPlayDataAsync();
        });
    }

    /// <inheritdoc/>
    protected override bool IsMediaLoaded()
        => true;

    /// <inheritdoc/>
    protected override Task OnLoadPlayDataAsync()
    {
        if (_isLoaded || _playerWindowHandle.Value == IntPtr.Zero)
        {
            return Task.CompletedTask;
        }

        _isLoaded = true;
        UpdateState(PlayerState.None);
        var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
        var referer = IsLive ? LiveReferer : VideoReferer;
        var token = this.Get<IBiliTokenResolver>().GetToken().AccessToken;
        var httpParams =
            IsWebDav ?
            $"--http-header-fields=\"Authorization: Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavConfig.UserName}:{_webDavConfig.Password}"))}\""
            : IsLive
                ? $"--cookies --no-ytdl --user-agent=\"{LiveUserAgent}\" --http-header-fields=\"Cookie: {cookies}\" --http-header-fields=\"Referer: {LiveReferer}\""
                : $"--cookies --user-agent=\"{VideoUserAgent}\" --http-header-fields=\"Cookie: {cookies}\" --http-header-fields=\"Referer: {VideoReferer}\"";
        var externalPlayerType = SettingsToolkit.ReadLocalSetting(SettingNames.ExternalPlayer, ExternalPlayerType.Mpv);
        var externalPlayer = externalPlayerType switch
        {
            ExternalPlayerType.Mpv => "mpv",
            ExternalPlayerType.MpvNet => "mpvnet",
            _ => throw new ArgumentOutOfRangeException(nameof(externalPlayerType)),
        };
        var command = httpParams;

        command += $" --wid={Convert.ToUInt32(_playerWindowHandle.Value)}";
        if (!string.IsNullOrEmpty(Title))
        {
            command += $" --title=\"{Title}\"";
        }

        if (!string.IsNullOrEmpty(_extraOptions))
        {
            command += $" --script-opts=\"cid={_extraOptions}\"";
        }

        if (Position > 0 && !IsLive)
        {
            command += $" --start={Position}";
        }

        if (!string.IsNullOrEmpty(_audioUrl) && string.IsNullOrEmpty(_videoUrl))
        {
            command += $" \"{_audioUrl}\"";
        }
        else
        {
            command += $" \"{_videoUrl}\"";
            if (!string.IsNullOrEmpty(_audioUrl))
            {
                command += $" --audio-file=\"{_audioUrl}\"";
            }
        }

        try
        {
            _ = Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = externalPlayer,
                        Arguments = command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    },
                };

                process.Start();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动外部播放器时发生错误.");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnSeekAsync(TimeSpan position)
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
    {
    }

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
    {
    }

    /// <inheritdoc/>
    protected override Task OnTakeScreenshotAsync(string path)
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override Task OnTogglePlayPauseAsync()
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override void SetWebDavConfig(WebDavConfig config)
    {
    }
}
