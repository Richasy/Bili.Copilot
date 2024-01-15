// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using WebDav;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 播放器页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPageViewModel"/> class.
    /// </summary>
    public WebDavPlayerPageViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        IsContinuePlay = SettingsToolkit.ReadLocalSetting(SettingNames.IsWebDavContinuePlay, true);
        Sections = new ObservableCollection<PlayerSectionHeader>();
        Playlist = new ObservableCollection<WebDavStorageItemViewModel>();
    }

    /// <summary>
    /// 设置关联的窗口.
    /// </summary>
    /// <param name="window">窗口实例.</param>
    public void SetWindow(object window)
        => _attachedWindow = window as Window;

    /// <summary>
    /// 设置播放列表.
    /// </summary>
    /// <param name="items">列表集合.</param>
    /// <param name="playIndex">初始播放条目索引.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SetPlaylistAsync(List<WebDavResource> items, int playIndex = 0)
    {
        IsReloading = true;

        try
        {
            var currentConfigId = SettingsToolkit.ReadLocalSetting(SettingNames.SelectedWebDav, string.Empty);
            var configList = await FileToolkit.ReadLocalDataAsync<List<WebDavConfig>>(AppConstants.WebDavConfigFileName, "[]");
            _config = configList.FirstOrDefault(p => p.Id == currentConfigId);

            ReloadMediaPlayer();
            TryClear(Playlist);
            foreach (var item in items)
            {
                Playlist.Add(new WebDavStorageItemViewModel(item));
            }

            TryClear(Sections);
            Sections.Add(new PlayerSectionHeader(PlayerSectionType.VideoInformation, ResourceToolkit.GetLocalizedString(StringNames.Information)));

            if (Playlist.Count > 0)
            {
                Sections.Add(new PlayerSectionHeader(PlayerSectionType.Playlist, ResourceToolkit.GetLocalizedString(StringNames.Playlist)));
            }

            CurrentSection = Sections.First();

            var selectedItem = Playlist[playIndex];
            ChangeVideoCommand.Execute(selectedItem);
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }

        IsReloading = false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Reset()
    {
        Playlist.Clear();
        ResetInfo();
        _isStatsUpdated = false;
    }

    [RelayCommand]
    private void Reload()
    {
        if (CurrentItem != null)
        {
            ChangeVideo(CurrentItem);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Reset();
        if (PlayerDetail != null)
        {
            PlayerDetail.MediaEnded -= OnMediaEnded;
            PlayerDetail.RequestOpenInBrowser -= OnRequestOpenInBrowserAsync;
            PlayerDetail.PropertyChanged -= OnPlayerDetailPropertyChanged;
            PlayerDetail?.Dispose();
        }
    }

    partial void OnCurrentSectionChanged(PlayerSectionHeader value)
    {
        if (value != null)
        {
            CheckSectionVisibility();
        }
    }

    partial void OnIsContinuePlayChanged(bool value)
        => SettingsToolkit.ReadLocalSetting(SettingNames.IsWebDavContinuePlay, value);
}
