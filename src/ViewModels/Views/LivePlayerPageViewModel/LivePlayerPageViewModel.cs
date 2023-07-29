// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播播放页面视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPageViewModel"/> class.
    /// </summary>
    public LivePlayerPageViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Danmakus = new ObservableCollection<LiveDanmakuInformation>();
        Sections = new ObservableCollection<PlayerSectionHeader>
        {
            new PlayerSectionHeader(PlayerSectionType.LiveInformation, ResourceToolkit.GetLocalizedString(StringNames.LiveInformation)),
            new PlayerSectionHeader(PlayerSectionType.Chat, ResourceToolkit.GetLocalizedString(StringNames.Chat)),
        };
        CurrentSection = Sections.First();
        IsSignedIn = AuthorizeProvider.Instance.State == AuthorizeState.SignedIn;
        AuthorizeProvider.Instance.StateChanged += OnAuthorizeStateChanged;

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, OpenInBroswerCommand);

        Danmakus.CollectionChanged += OnDanmakusCollectionChanged;
    }

    /// <summary>
    /// 设置播放快照.
    /// </summary>
    public void SetSnapshot(PlaySnapshot snapshot)
    {
        ReloadMediaPlayer();
        _presetRoomId = snapshot.VideoId;
        var defaultPlayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        _ = ReloadCommand.ExecuteAsync(null)
            .ContinueWith(_ =>
            {
                _ = _dispatcherQueue.TryEnqueue(() =>
                {
                    PlayerDetail.DisplayMode = snapshot.DisplayMode ?? defaultPlayMode;
                });
            });

        LiveProvider.Instance.MessageReceived += OnMessageReceived;
    }

    /// <summary>
    /// 设置关联的窗口.
    /// </summary>
    /// <param name="window">窗口实例.</param>
    public void SetWindow(object window)
        => _attachedWindow = window as Window;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [RelayCommand]
    private void Reset()
    {
        View = null;
        ResetTimers();
        ResetPublisher();
        ResetOverview();
        ResetInterop();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        Reset();
        View = await LiveProvider.GetLiveRoomDetailAsync(_presetRoomId);

        var isEnterSuccess = await LiveProvider.Instance.EnterLiveRoomAsync(_presetRoomId);
        if (isEnterSuccess)
        {
            InitializeTimers();
            InitializePublisher();
            InitializeOverview();

            PlayerDetail.SetLiveData(View);
        }
        else
        {
            DisplayException(new Exception("进入直播间失败"));
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Reset();
                PlayerDetail.RequestOpenInBrowser -= OnRequestOpenInBrowserAsync;
                PlayerDetail.PropertyChanged -= OnPlayerDetailPropertyChanged;
                PlayerDetail?.Dispose();
            }

            _disposedValue = true;
        }
    }

    partial void OnCurrentSectionChanged(PlayerSectionHeader value)
    {
        CheckSectionVisibility();
    }
}
