﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Player;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    private void ResetLiveData()
        => _currentPlayline = default;

    private async Task LoadLiveAsync()
    {
        await InitializeLiveMediaInformationAsync();
        await InitializeOriginalLiveSourceAsync();
    }

    private async Task InitializeLiveMediaInformationAsync()
    {
        var view = _viewData as LivePlayerView;
        var quality = _currentPlayline != null
            ? _currentPlayline.Quality
            : SettingsToolkit.ReadLocalSetting(SettingNames.DefaultLiveFormat, 400);

        Cover = view.Information.Identifier.Cover.GetSourceUri().ToString();
        DanmakuViewModel.SetData(view.Information.Identifier.Id, default, _videoType);
        _liveMediaInformation = await LiveProvider.GetLiveMediaInformationAsync(view.Information.Identifier.Id, quality, IsLiveAudioOnly);

        if (_currentPlayline == null)
        {
            _currentPlayline = _liveMediaInformation.Lines.FirstOrDefault(p => p.Quality == quality) ?? _liveMediaInformation.Lines.First();
        }
    }

    private async Task InitializeOriginalLiveSourceAsync()
    {
        var isVip = AccountViewModel.Instance.IsVip;
        if (isVip)
        {
            foreach (var item in _liveMediaInformation.Formats)
            {
                item.IsLimited = false;
            }
        }

        foreach (var item in _liveMediaInformation.Formats)
        {
            if (!item.IsLimited)
            {
                Formats.Add(item);
            }
        }

        var formatId = GetFormatId(true);
        await SelectLiveFormatAsync(Formats.First(p => p.Quality == formatId));
    }

    private async Task SelectLiveFormatAsync(FormatInformation format)
    {
        CurrentFormat = format;
        ResetPlayer();
        InitializePlayer();
        var view = _viewData as LivePlayerView;
        var codecId = GetLivePreferCodecId();
        var quality = format.Quality;
        _liveMediaInformation = await LiveProvider.GetLiveMediaInformationAsync(view.Information.Identifier.Id, quality, IsLiveAudioOnly);
        if (_liveMediaInformation.Lines != null)
        {
            var playlines = _liveMediaInformation.Lines.Where(p => p.Name == codecId);
            if (playlines.Count() == 0)
            {
                playlines = _liveMediaInformation.Lines.Where(p => p.Urls.Any(j => j.Host.EndsWith(".com")));
            }

            var url = playlines.SelectMany(p => p.Urls).FirstOrDefault(p => p.Host.EndsWith(".com"));
            if (url == null)
            {
                IsError = true;
                ErrorText = ResourceToolkit.GetLocalizedString(StringNames.FlvNotSupported);
                return;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultLiveFormat, CurrentFormat.Quality);
            InitializeLivePlayer(url.ToString());
        }
    }

    private void InitializeLivePlayer(string url)
    {
        Player.SetLiveSource(url);
        StartTimers();
    }

    private async Task ChangeLiveAudioOnlyAsync(bool isAudioOnly)
    {
        IsLiveAudioOnly = isAudioOnly;
        SettingsToolkit.WriteLocalSetting(SettingNames.IsLiveAudioOnly, isAudioOnly);
        if (CurrentFormat != null)
        {
            await SelectLiveFormatAsync(CurrentFormat);
        }
    }

    private void FillLivePlaybackProperties()
    {
        var view = _viewData as LivePlayerView;
        SetDisplayProperties(
            view.Information.User.Avatar.GetSourceUri() + "@100w_100h_1c_100q.jpg",
            view.Information.Identifier.Title,
            string.Join(string.Empty, view.Information.Description.Take(20)),
            _videoType.ToString());
    }
}
