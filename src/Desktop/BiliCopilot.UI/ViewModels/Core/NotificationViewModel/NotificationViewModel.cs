// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationViewModel"/> class.
    /// </summary>
    public NotificationViewModel(
        IMomentDiscoveryService momentService,
        ILogger<NotificationViewModel> logger)
    {
        _momentService = momentService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task TryStartAsync()
    {
        var isNotifyEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsNotificationEnabled, true);
        if (!isNotifyEnabled)
        {
            return;
        }

        if (_timer is null)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(15);
            _timer.Tick += OnTimerTick;
        }

        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }

        await InitializeCommand.ExecuteAsync(default);
        TrySendVideoMomentNotificationsCommand.Execute(default);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var entry = (await Package.Current.GetAppListEntriesAsync()).First();
        _isTileSupport = StartScreenManager.GetDefault().SupportsAppListEntry(entry);

        if (_isTileSupport)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }
    }

    private void OnTimerTick(object? sender, object e)
        => TrySendVideoMomentNotificationsCommand.Execute(default);
}
