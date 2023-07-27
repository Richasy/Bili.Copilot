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
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 播放页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPageViewModel"/> class.
    /// </summary>
    public PgcPlayerPageViewModel()
    {
        FavoriteFolders = new ObservableCollection<VideoFavoriteFolderSelectableViewModel>();
        Sections = new ObservableCollection<PlayerSectionHeader>();
        Episodes = new ObservableCollection<EpisodeItemViewModel>();
        Seasons = new ObservableCollection<VideoIdentifierSelectableViewModel>();
        Extras = new ObservableCollection<PgcExtraItemViewModel>();
        Celebrities = new ObservableCollection<UserItemViewModel>();

        IsSignedIn = AuthorizeProvider.Instance.State == AuthorizeState.SignedIn;
        AuthorizeProvider.Instance.StateChanged += OnAuthorizeStateChanged;

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachIsRunningToAsyncCommand(p => IsFavoriteFolderRequesting = p, RequestFavoriteFoldersCommand);

        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayFavoriteFoldersException, RequestFavoriteFoldersCommand);

        PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// 设置播放快照.
    /// </summary>
    public void SetSnapshot(PlaySnapshot snapshot)
    {
        ReloadMediaPlayer();
        _presetEpisodeId = string.IsNullOrEmpty(snapshot.VideoId)
            ? "0"
            : snapshot.VideoId;
        _presetSeasonId = string.IsNullOrEmpty(snapshot.SeasonId)
            ? "0"
            : snapshot.SeasonId;
        _presetTitle = snapshot.Title;
        _needBiliPlus = snapshot.NeedBiliPlus;
        var defaultPlayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        PlayerDetail.DisplayMode = snapshot.DisplayMode ?? defaultPlayMode;
        ReloadCommand.ExecuteAsync(default);
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
    private void Clear()
    {
        View = null;
        ResetOverview();
        ResetOperation();
        ResetCommunityInformation();
        ResetInterop();
        ResetSections();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        Clear();
        if (_needBiliPlus && !string.IsNullOrEmpty(_presetEpisodeId))
        {
            var data = await PgcProvider.GetBiliPlusBangumiInformationAsync(_presetEpisodeId);
            if (data != null)
            {
                var epId = data.PlayUrl.Split('/').Last();
                _presetEpisodeId = !string.IsNullOrEmpty(epId) && epId.Contains("ep")
                            ? epId.Replace("ep", string.Empty)
                            : "0";
                _presetSeasonId = data.SeasonId;
            }
        }

        var proxyPack = AppToolkit.GetProxyAndArea(_presetTitle, false);
        View = await PlayerProvider.GetPgcDetailAsync(_presetEpisodeId, _presetSeasonId, proxyPack.Item1, proxyPack.Item2);

        InitializeOverview();
        InitializeOperation();
        InitializeCommunityInformation();
        InitializeSections();
        InitializeInterop();

        PlayerDetail.SetPgcData(View, CurrentEpisode);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Clear();
                if (PlayerDetail != null)
                {
                    PlayerDetail.MediaEnded -= OnMediaEnded;
                    PlayerDetail.InternalPartChanged -= OnInternalPartChanged;
                    PlayerDetail.RequestOpenInBrowser -= OnRequestOpenInBrowserAsync;
                    PlayerDetail?.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    private void ReloadMediaPlayer()
    {
        if (PlayerDetail != null)
        {
            return;
        }

        PlayerDetail = new PlayerDetailViewModel(_attachedWindow);
        PlayerDetail.MediaEnded += OnMediaEnded;
        PlayerDetail.InternalPartChanged += OnInternalPartChanged;
        PlayerDetail.RequestOpenInBrowser += OnRequestOpenInBrowserAsync;
    }
}
