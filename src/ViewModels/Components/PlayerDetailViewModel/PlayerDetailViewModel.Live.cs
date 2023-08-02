// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    /// <summary>
    /// 获取当前的直播播放地址.
    /// </summary>
    /// <returns>直播播放地址.</returns>
    public string GetLivePlayUrl()
        => _currentLiveUrl == null ? "--" : $"{_currentLiveUrl.Host}{_currentLiveUrl.Route}";

    /// <summary>
    /// 获取当前的视频播放地址.
    /// </summary>
    /// <returns>播放地址.</returns>
    public string GetVideoPlayUrl()
        => _video == null ? "--" : new Uri(_video.BaseUrl).Host;

    /// <summary>
    /// 获取当前的音频播放地址.
    /// </summary>
    /// <returns>播放地址.</returns>
    public string GetAudioPlayUrl()
        => _audio == null ? "--" : new Uri(_audio.BaseUrl).Host;

    private void ResetLiveData()
        => _currentPlayLine = default;

    private async Task LoadLiveAsync()
    {
        await InitializeLiveMediaInformationAsync();
        await InitializeOriginalLiveSourceAsync();
    }

    private async Task InitializeLiveMediaInformationAsync()
    {
        var view = _viewData as LivePlayerView;
        var quality = _currentPlayLine != null
            ? _currentPlayLine.Quality
            : SettingsToolkit.ReadLocalSetting(SettingNames.DefaultLiveFormat, 400);

        Cover = view.Information.Identifier.Cover.GetSourceUri().ToString();
        DanmakuViewModel.SetData(view.Information.Identifier.Id, default, _videoType);
        _liveMediaInformation = await LiveProvider.GetLiveMediaInformationAsync(view.Information.Identifier.Id, quality, IsLiveAudioOnly);

        _currentPlayLine ??= _liveMediaInformation.Lines.FirstOrDefault(p => p.Quality == quality) ?? _liveMediaInformation.Lines.First();
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
            var playLines = _liveMediaInformation.Lines.Where(p => p.Name == codecId);
            if (playLines.Count() == 0)
            {
                playLines = _liveMediaInformation.Lines.Where(p => p.Urls.Any(j => j.Protocol is "http_hls" or "http_stream"));
            }

            var url = playLines.SelectMany(p => p.Urls).FirstOrDefault(p => p.Protocol == "http_hls");
            if (url == null)
            {
                url = playLines.SelectMany(p => p.Urls).FirstOrDefault(p => p.Protocol == "http_stream");
                if (url == null)
                {
                    IsError = true;
                    ErrorText = ResourceToolkit.GetLocalizedString(StringNames.FlvNotSupported);
                    return;
                }
            }

            _currentLiveUrl = url;
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultLiveFormat, CurrentFormat.Quality);
            InitializeLivePlayer(url.ToString());
        }
    }

    private void InitializeLivePlayer(string url)
    {
        Player.SetLiveSource(url, IsLiveAudioOnly);
        StartTimers();
    }

    [RelayCommand]
    private async Task ChangeLiveAudioOnlyAsync(bool isAudioOnly)
    {
        IsLiveAudioOnly = isAudioOnly;
        SettingsToolkit.WriteLocalSetting(SettingNames.IsLiveAudioOnly, isAudioOnly);
        if (CurrentFormat != null)
        {
            await SelectLiveFormatAsync(CurrentFormat);
        }
    }
}
