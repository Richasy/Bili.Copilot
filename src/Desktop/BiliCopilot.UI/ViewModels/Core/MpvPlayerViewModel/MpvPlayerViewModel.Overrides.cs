// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;
using Richasy.MpvKernel.WinUI;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class MpvPlayerViewModel
{
    private const int WindowMinWidth = 640;
    private const int WindowMinHeight = 480;

    private MpvPlayerWindow? _playerWindow;
    private MpvPlayerState _lastState = MpvPlayerState.Idle;

    public MpvClient? Client { get; private set; }

    /// <inheritdoc/>
    protected override void BeforeLoadPlayData()
    {
    }

    /// <inheritdoc/>
    protected override Task OnCloseAsync()
    {
        if (Client != null)
        {
            Client.DataNotify -= OnDataNotify;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override bool IsMediaLoaded()
        => _lastState != MpvPlayerState.Idle;

    /// <inheritdoc/>
    protected override async Task OnTogglePlayPauseAsync()
    {
        if (_lastState == MpvPlayerState.Playing)
        {
            await Client.PauseAsync();
        }
        else if (_lastState == MpvPlayerState.Paused)
        {
            await Client.ResumeAsync();
        }
        else if (_lastState == MpvPlayerState.Idle)
        {
            await Client.ReplayAsync();
        }
    }

    /// <inheritdoc/>
    protected override async Task ForcePlayAsync()
    {
        await Client.ResumeAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnSeekAsync(TimeSpan position)
    {
        await Client?.SetCurrentPositionAsync(position.TotalSeconds);
    }

    /// <inheritdoc/>
    protected override void OnSetVolume(int value)
    {
        _ = Client?.SetVolumeAsync(value);
    }

    /// <inheritdoc/>
    protected override void OnSetSpeed(double speed)
    {
        _ = Client?.SetSpeedAsync(speed);
        _speedAction?.Invoke(speed);
    }

    /// <inheritdoc/>
    protected override Task OnTakeScreenshotAsync(string path)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task OnLoadPlayDataAsync()
    {
        var options = new MpvPlayOptions();
        var headers = new Dictionary<string, string>();
        if (IsWebDav && _webDavConfig != null)
        {
            var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavConfig.UserName}:{_webDavConfig.Password}"))}";
            headers.Add("Authorization", auth);
        }
        else if (!IsWebDav)
        {
            var cookies = this.Get<IBiliCookiesResolver>().GetCookieString();
            var referer = IsLive ? LiveReferer : VideoReferer;
            var userAgent = IsLive ? LiveUserAgent : VideoUserAgent;
            headers.Add("Cookie", cookies);
            headers.Add("Referer", referer);
            options.UserAgent = userAgent;
            options.EnableCookies = true;
        }

        options.HttpHeaders = headers;
        if (IsLive || IsWebDav)
        {
            options.EnableYtdl = false;
        }

        UpdateState(Models.Constants.PlayerState.None);
        options.WindowHandle = _playerWindow.Handle;
        options.StartPosition = Position;
        options.InitialVolume = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PlayerVolume, 100);
        options.InitialSpeed = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PlayerSpeed, 1d);
        var fileUrl = string.Empty;
        if (!string.IsNullOrEmpty(_videoUrl))
        {
            fileUrl = _videoUrl;
            if (!string.IsNullOrEmpty(_audioUrl))
            {
                options.ExtraAudioUrl = _audioUrl;
            }
        }
        else if (!string.IsNullOrEmpty(_audioUrl))
        {
            fileUrl = _audioUrl;
        }

        await Client?.PlayAsync(fileUrl, options);
    }

    /// <inheritdoc/>
    protected override void SetWebDavConfig(WebDavConfig config)
    {
        if (IsWebDav && Client != null)
        {
            var auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_webDavConfig.UserName}:{_webDavConfig.Password}"))}";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", auth }
            };

            // TODO: 设置选项.
        }
    }
}
