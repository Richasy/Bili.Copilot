// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 播放器页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPageViewModel"/> class.
    /// </summary>
    public WebDavPlayerPageViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Playlist = new System.Collections.ObjectModel.ObservableCollection<Items.WebDavStorageItemViewModel>();
    }

    [RelayCommand]
    private async Task InitializeAsync(List<WebDavStorageItemViewModel> items)
    {
        OnlyOne = items.Count == 1;
        var currentConfigId = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.SelectedWebDav, string.Empty);
        var configList = await FileToolkit.ReadLocalDataAsync<List<WebDavConfig>>(AppConstants.WebDavConfigFileName, "[]");
        _config = configList.FirstOrDefault(p => p.Id == currentConfigId);

        var selectedItemId = items.First(p => p.IsSelected).Data.Href;
        var dataList = items.Select(p => p.Data);
        foreach (var item in dataList)
        {
            Playlist.Add(new WebDavStorageItemViewModel(item));
        }

        var selectedItem = Playlist.FirstOrDefault(p => p.Data.Href == selectedItemId);
        if (selectedItem is not null)
        {
            selectedItem.IsSelected = true;
        }

        PlayCommand.Execute(selectedItem);
    }

    [RelayCommand]
    private async Task PlayAsync(WebDavStorageItemViewModel item)
    {
        if (SelectedItem != null && SelectedItem.Equals(item))
        {
            return;
        }

        SelectedItem = item;
        foreach (var data in Playlist)
        {
            data.IsSelected = data.Equals(item);
        }

        if (_stream != null)
        {
            _stream?.Dispose();
        }

        var handler = new HttpClientHandler();
        handler.Credentials = new NetworkCredential(_config.UserName, _config.Password);
        handler.PreAuthenticate = true;
        if (handler.SupportsAutomaticDecompression)
        {
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        var httpClient = new HttpClient(handler);
        _stream = await HttpRandomAccessStream.CreateAsync(httpClient, new System.Uri(_config.Host + ":" + _config.Port + SelectedItem.Data.Href));
        var source = MediaSource.CreateFromStream(_stream, SelectedItem.Data.ContentType);

        Player ??= GetMediaPlayer();
        Player.Source = new MediaPlaybackItem(source);
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (Player == null || Player.PlaybackSession == null)
        {
            return;
        }

        if (Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
        {
            Player.Pause();
        }
        else
        {
            Player.Play();
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Playlist.Clear();
        SelectedItem = null;
        _stream?.Dispose();
        Player?.Dispose();
        Player = null;
    }

    private MediaPlayer GetMediaPlayer()
    {
        var player = new MediaPlayer();
        player.CurrentStateChanged += OnMediaStateChanged;
        return player;
    }

    private void OnMediaStateChanged(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (sender.CurrentState == MediaPlayerState.Stopped)
            {
                var position = sender.Position;

                // 判断为已经播放完成，切换到下一个视频.
                if (Math.Abs(position.TotalSeconds - sender.NaturalDuration.TotalSeconds) < 1)
                {
                    var isAuto = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.WebDavAutoNext, true);
                    if (isAuto)
                    {
                        var index = Playlist.IndexOf(SelectedItem);
                        if (index < Playlist.Count - 1)
                        {
                            PlayCommand.Execute(Playlist[index + 1]);
                        }
                    }
                }
            }
        });
    }
}
