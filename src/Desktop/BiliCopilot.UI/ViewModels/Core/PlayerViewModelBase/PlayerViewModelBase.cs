// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.WinUI.Share.ViewModels;
using Windows.Media;
using Windows.Storage.Streams;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型基类.
/// </summary>
public abstract partial class PlayerViewModelBase : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerViewModelBase"/> class.
    /// </summary>
    protected PlayerViewModelBase()
    {
        _logger = this.Get<ILogger<PlayerViewModelBase>>();
        _dispatcherQueue = this.Get<DispatcherQueue>();
    }

    /// <summary>
    /// 初始化系统播放控制器.
    /// </summary>
    public void InitializeSmtc(string cover, string title, string subtitle)
    {
        if (_smtc is null)
        {
            return;
        }

        var updater = _smtc.DisplayUpdater;
        updater.ClearAll();
        updater.Type = MediaPlaybackType.Video;
        if (!string.IsNullOrEmpty(cover))
        {
            updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(cover));
        }

        updater.VideoProperties.Title = title;
        updater.VideoProperties.Subtitle = subtitle;
        updater.Update();
    }

    /// <summary>
    /// 注入 WebDav 配置.
    /// </summary>
    public void InjectWebDavConfig(WebDavConfig config)
    {
        _webDavConfig = config;
        SetWebDavConfig(config);
    }

    /// <summary>
    /// 设置播放数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public virtual async Task SetPlayDataAsync(string? videoUrl, string? audioUrl, bool isAutoPlay, int position = 0, string? contentType = default)
    {
        if (_smtc is null)
        {
            var currentWindow = this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle();
            _smtc = SystemMediaTransportControlsInterop.GetForWindow(currentWindow);
            _smtc.IsEnabled = true;
            _smtc.IsPlayEnabled = true;
            _smtc.IsPauseEnabled = true;
            _smtc.ButtonPressed += OnSystemControlsButtonPressedAsync;

            // 独立窗口播放时默认全窗口播放.
            if (IsSeparatorWindowPlayer && !IsFullWindow)
            {
                ToggleFullWindowCommand.Execute(default);
            }
            else
            {
                var defaultDisplay = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
                if (!IsFullScreen && defaultDisplay == PlayerDisplayMode.FullScreen)
                {
                    ToggleFullScreenCommand.Execute(default);
                }
                else if (!IsCompactOverlay && defaultDisplay == PlayerDisplayMode.CompactOverlay)
                {
                    ToggleCompactOverlayCommand.Execute(default);
                }
                else if (!IsFullWindow && defaultDisplay == PlayerDisplayMode.FullWindow)
                {
                    ToggleFullWindowCommand.Execute(default);
                }
            }
        }

        if (IsMediaLoaded() && !IsPaused)
        {
            await TogglePlayPauseAsync();
        }

        _videoUrl = videoUrl;
        _audioUrl = audioUrl;
        _autoPlay = isAutoPlay;
        Position = position;
        _contentType = contentType;

        CancelNotification();

        var isSpeedShare = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedShare, true);
        var localSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerSpeed, 1.0);
        Volume = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerVolume, 100);
        Speed = isSpeedShare ? localSpeed : 1.0;
        MaxSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedEnhancement, false) ? 6.0 : 3.0;

        if (_isInitialized)
        {
            BeforeLoadPlayData();
            await TryLoadPlayDataAsync();
        }
    }

    /// <summary>
    /// 注入进度改变时的回调.
    /// </summary>
    public void SetProgressAction(Action<int, int> action)
        => _progressAction = action;

    /// <summary>
    /// 注入状态改变时的回调.
    /// </summary>
    public void SetStateAction(Action<PlayerState> action)
        => _stateAction = action;

    /// <summary>
    /// 注入播放结束时的回调.
    /// </summary>
    public void SetEndAction(Action action)
        => _endAction = action;

    /// <summary>
    /// 注入重新加载时的回调.
    /// </summary>
    public void SetReloadAction(Action action)
        => _reloadAction = action;

    /// <summary>
    /// 关闭播放器.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public Task CloseAsync()
    {
        IsPaused = true;
        if (_smtc is not null)
        {
            _smtc.DisplayUpdater.ClearAll();
            _smtc.IsEnabled = false;
            _smtc.ButtonPressed -= OnSystemControlsButtonPressedAsync;
            _smtc = default;
        }

        return OnCloseAsync();
    }

    /// <summary>
    /// 显示通知.
    /// </summary>
    public void ShowNotification(PlayerNotification notification)
    {
        var item = new PlayerNotificationItemViewModel(notification);
        RequestShowNotification?.Invoke(this, item);
    }

    /// <summary>
    /// 取消通知.
    /// </summary>
    public void CancelNotification()
        => RequestCancelNotification?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 检查底部进度条是否可见.
    /// </summary>
    /// <param name="shouldShow">是否需要显示.</param>
    public void CheckBottomProgressVisibility(bool shouldShow)
    {
        var isEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsBottomProgressVisible, true);
        IsBottomProgressVisible = isEnabled && shouldShow && !IsLive;
    }

    /// <summary>
    /// 在加载播放数据之前.
    /// </summary>
    protected virtual void BeforeLoadPlayData()
    {
    }

    /// <summary>
    /// 准备关闭.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected virtual Task OnCloseAsync()
        => Task.CompletedTask;

    /// <summary>
    /// 尝试加载播放数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    protected abstract Task OnLoadPlayDataAsync();

    partial void OnIsFullScreenChanged(bool value)
    {
        if (value && IsCompactOverlay)
        {
            IsCompactOverlay = false;
        }
        else if (value && IsFullWindow)
        {
            IsFullWindow = false;
        }
    }

    partial void OnIsCompactOverlayChanged(bool value)
    {
        if (value && IsFullScreen)
        {
            IsFullScreen = false;
        }
        else if (value && IsFullWindow)
        {
            IsFullWindow = false;
        }
    }

    partial void OnIsFullWindowChanged(bool value)
    {
        if (value && IsFullScreen)
        {
            IsFullScreen = false;
        }
        else if (value && IsCompactOverlay)
        {
            IsCompactOverlay = false;
        }
    }
}
