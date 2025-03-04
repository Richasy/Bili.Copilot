﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 外部播放器视图模型.
/// </summary>
public sealed partial class ExternalPlayerViewModel
{
    /// <inheritdoc/>
    protected override void SetWebDavConfig(WebDavConfig config)
    {
    }

    /// <inheritdoc/>
    protected override void BeforeLoadPlayData()
    {
    }

    /// <inheritdoc/>
    protected override Task OnCloseAsync()
    {
        ResetPlayer();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override bool IsMediaLoaded()
        => PlayerProcess != null && !PlayerProcess.HasExited;

    /// <inheritdoc/>
    protected override Task OnTogglePlayPauseAsync()
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override Task OnSeekAsync(TimeSpan position)
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override Task ForcePlayAsync()
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
    {
    }

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
    {
    }

    /// <inheritdoc/>
    protected override Task OnTakeScreenshotAsync(string path)
        => Task.CompletedTask;

    /// <inheritdoc/>
    protected override Task OnLoadPlayDataAsync()
    {
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
            ResetPlayer();
            RaiseInitializedEvent();
            _ = Task.Run(() =>
            {
                PlayerProcess = new Process
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
                        StandardErrorEncoding = Encoding.UTF8,
                        StandardOutputEncoding = Encoding.UTF8,
                    },
                };

                PlayerProcess.OutputDataReceived += OnPlayerProcessDataReceived;
                PlayerProcess.ErrorDataReceived += OnPlayerProcessDataReceived;
                PlayerProcess.Start();
                PlayerProcess.BeginOutputReadLine();
                PlayerProcess.BeginErrorReadLine();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动外部播放器时发生错误.");
        }

        return Task.CompletedTask;
    }

    private void OnPlayerProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        var msg = e.Data;
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }

        if (msg.Contains("V:") || msg.Contains("A:"))
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                if (msg.Contains("Paused"))
                {
                    UpdateState(PlayerState.Paused);
                }
                else if (msg.Contains("Buffering"))
                {
                    UpdateState(PlayerState.Buffering);
                }
                else if (msg.Contains("(Quit)"))
                {
                    UpdateState(PlayerState.None);
                }
                else
                {
                    UpdateState(PlayerState.Playing);
                }
            });

            ParsePositionAndDuration();
        }
        else
        {
            if (_mpvDebugMessages.Count > 0 && _mpvDebugMessages.Last() == msg)
            {
                return;
            }

            if (_mpvDebugMessages.Count >= 20)
            {
                _mpvDebugMessages.RemoveAt(0);
            }

            _mpvDebugMessages.Add(msg);
            _dispatcherQueue.TryEnqueue(() =>
            {
                LogMessage = string.Join('\n', _mpvDebugMessages);
            });
        }

        bool ParsePositionAndDuration()
        {
            var position = TimeSpan.Zero;
            var duration = TimeSpan.Zero;
            var pattern = @"AV:\s*(\d{2}:\d{2}:\d{2}\.\d{3})\s*/\s*(\d{2}:\d{2}:\d{2}\.\d{3})";

            var match = Regex.Match(msg, pattern);
            if (match.Success)
            {
                position = TimeSpan.Parse(match.Groups[1].Value);
                duration = TimeSpan.Parse(match.Groups[2].Value);

                _dispatcherQueue.TryEnqueue(() =>
                {
                    UpdatePosition((int)position.TotalSeconds, (int)duration.TotalSeconds);
                });
            }

            return match.Success;
        }
    }
}
