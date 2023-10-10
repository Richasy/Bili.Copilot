// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    private void ResetVideoData()
        => _currentPart = default;

    private async Task ChangeVideoPartAsync(VideoIdentifier part)
    {
        var view = _viewData as VideoPlayerView;
        if (string.IsNullOrEmpty(part.Id) || (!IsInteractionVideo && !view.SubVideos.Contains(part)))
        {
            return;
        }

        _currentPart = part;
        _shouldMarkProgress = false;
        ResetMediaData();
        await LoadVideoAsync();
        StartTimers();
    }

    private async Task LoadVideoAsync()
    {
        InitializeVideoInformation();
        await InitializeVideoMediaInformationAsync();
        CheckVideoHistory();
        await InitializeOriginalVideoSourceAsync();

        var view = _viewData as VideoPlayerView;
        SubtitleViewModel.SetData(view.Information.Identifier.Id, _currentPart.Id);
        DanmakuViewModel.SetData(view.Information.Identifier.Id, _currentPart.Id, _videoType);
        DownloadViewModel.SetData("av" + view.Information.Identifier.Id, view.SubVideos);
    }

    private void CheckVideoHistory()
    {
        var view = _viewData as VideoPlayerView;
        if (view.Progress != null && view.Progress.Status == PlayedProgressStatus.Playing)
        {
            var history = view.Progress.Identifier;

            var ts = TimeSpan.FromSeconds(view.Progress.Progress);
            IsShowProgressTip = true;
            ProgressTip = $"{ResourceToolkit.GetLocalizedString(StringNames.PreviousView)}{history.Title} {ts}";
        }
    }

    private void InitializeVideoInformation()
    {
        var view = _viewData as VideoPlayerView;
        Cover = view.Information.Identifier.Cover.GetSourceUri().ToString();
        IsInteractionVideo = view.InteractionVideo != null;
        if (string.IsNullOrEmpty(_currentPart.Id))
        {
            _currentPart = view.SubVideos.First();
            if (IsInteractionVideo)
            {
                InteractionViewModel.SetData(view.Information.Identifier.Id, default, view.InteractionVideo.GraphVersion);
            }
        }
    }

    private async Task InitializeVideoMediaInformationAsync()
    {
        var view = _viewData as VideoPlayerView;
        _mediaInformation = await PlayerProvider.GetVideoMediaInformationAsync(view.Information.Identifier.Id, _currentPart.Id);
        CheckVideoP2PUrls();
    }

    private async Task InitializeOriginalVideoSourceAsync()
    {
        var isVip = AccountViewModel.Instance.IsVip;
        if (isVip)
        {
            foreach (var item in _mediaInformation.Formats)
            {
                item.IsLimited = false;
            }
        }

        foreach (var item in _mediaInformation.Formats)
        {
            if (!item.IsLimited)
            {
                Formats.Add(item);
            }
        }

        var formatId = GetFormatId();
        await SelectVideoFormatAsync(Formats.First(p => p.Quality == formatId));
    }

    private async Task SelectVideoFormatAsync(FormatInformation format)
    {
        MarkProgressBreakpoint();
        var codecId = GetVideoPreferCodecId();
        ResetPlayer();
        await Task.Delay(400);
        InitializePlayer();
        if (_mediaInformation.VideoSegments != null)
        {
            var filteredSegments = _mediaInformation.VideoSegments.Where(p => p.Id == format.Quality.ToString());
            if (!filteredSegments.Any())
            {
                var maxQuality = _mediaInformation.VideoSegments.Max(p => Convert.ToInt32(p.Id));
                _video = _mediaInformation.VideoSegments.First(p => p.Id == maxQuality.ToString());
            }
            else
            {
                _video = filteredSegments.FirstOrDefault(p => p.Codecs.Contains(codecId))
                    ?? filteredSegments.First();
            }

            CurrentFormat = Formats.FirstOrDefault(p => p.Quality.ToString() == _video.Id);
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultVideoFormat, CurrentFormat.Quality);
        }

        if (_mediaInformation.AudioSegments != null)
        {
            var audioQuality = SettingsToolkit.ReadLocalSetting(SettingNames.PreferAudioQuality, PreferAudio.Standard);
            if (audioQuality == PreferAudio.HighQuality)
            {
                var maxBandWidth = _mediaInformation.AudioSegments.Max(p => p.Bandwidth);
                _audio = _mediaInformation.AudioSegments.First(p => p.Bandwidth == maxBandWidth);
            }
            else if (audioQuality == PreferAudio.Near)
            {
                _audio = _mediaInformation.AudioSegments.OrderBy(p => Math.Abs(p.Bandwidth - _video.Bandwidth)).First();
            }
            else if (audioQuality == PreferAudio.Standard)
            {
                _audio = _mediaInformation.AudioSegments.Where(p => p.Bandwidth < 100000).FirstOrDefault()
                    ?? _mediaInformation.AudioSegments.OrderBy(p => p.Bandwidth).First();
            }
        }

        if (_video == null)
        {
            IsError = true;
            ErrorText = ResourceToolkit.GetLocalizedString(StringNames.SourceNotSupported);
            return;
        }

        try
        {
            await Player.SetSourceAsync(_video, _audio, IsAudioOnly);
            StartTimers();
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorText = ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFailed);
            LogException(ex);
        }
    }

    private void CheckVideoP2PUrls()
    {
        if (!SettingsToolkit.ReadLocalSetting(SettingNames.DisableP2PCdn, false))
        {
            return;
        }

        // 剔除 P2P CDN URL
        if (_mediaInformation.AudioSegments != null)
        {
            var cdnUrl = _mediaInformation.AudioSegments.Select(p => p.BaseUrl)
                .Concat(_mediaInformation.AudioSegments.SelectMany(p => p.BackupUrls))
                .FirstOrDefault(p => Regex.IsMatch(p, "up[\\w-]+\\.bilivideo\\.com"));
            var cdn = string.IsNullOrEmpty(cdnUrl) ? "upos-sz-mirrorcoso1.bilivideo.com" : new Uri(cdnUrl).Host;

            for (var i = 0; i < _mediaInformation.AudioSegments.Count(); i++)
            {
                var seg = _mediaInformation.AudioSegments[i];
                var url = new Uri(seg.BaseUrl);
                if (url.Host.Contains(".mcdn.bilivideo.cn"))
                {
                    var replacedUrl = seg.BaseUrl.Replace(url.Host, cdn)
                        .Replace($":{url.Port}", ":443");

                    seg.BaseUrl = replacedUrl;
                }
            }
        }

        if (_mediaInformation.VideoSegments != null)
        {
            var cdnUrl = _mediaInformation.VideoSegments.Select(p => p.BaseUrl)
                .Concat(_mediaInformation.VideoSegments.SelectMany(p => p.BackupUrls))
                .FirstOrDefault(p => Regex.IsMatch(p, "up[\\w-]+\\.bilivideo\\.com"));
            var cdn = string.IsNullOrEmpty(cdnUrl) ? "upos-sz-mirrorcoso1.bilivideo.com" : new Uri(cdnUrl).Host;

            for (var i = 0; i < _mediaInformation.VideoSegments.Count(); i++)
            {
                var seg = _mediaInformation.VideoSegments[i];
                var url = new Uri(seg.BaseUrl);
                if (url.Host.Contains(".mcdn.bilivideo.cn"))
                {
                    var replacedUrl = seg.BaseUrl.Replace(url.Host, cdn)
                        .Replace($":{url.Port}", ":443");

                    seg.BaseUrl = replacedUrl;
                }
            }
        }
    }

    [RelayCommand]
    private void SelectInteractionChoice(InteractionInformation info)
    {
        IsShowInteractionChoices = false;
        IsInteractionEnd = false;
        if (_videoType != VideoType.Video)
        {
            return;
        }

        if (_viewData is not VideoPlayerView view || view.InteractionVideo == null)
        {
            return;
        }

        InteractionViewModel.SetData(view.Information.Identifier.Id, info.Id, view.InteractionVideo.GraphVersion);
        var part = new VideoIdentifier(info.PartId, default, default, default);
        _ = ChangePartCommand.ExecuteAsync(part);
    }

    [RelayCommand]
    private void BackToInteractionVideoStart()
    {
        IsShowInteractionChoices = false;
        IsInteractionEnd = false;
        if (_viewData is not VideoPlayerView view || view.InteractionVideo == null)
        {
            return;
        }

        InteractionViewModel.SetData(view.Information.Identifier.Id, default, view.InteractionVideo.GraphVersion);
        var part = view.SubVideos.FirstOrDefault();
        _ = ChangePartCommand.ExecuteAsync(part);
    }
}
