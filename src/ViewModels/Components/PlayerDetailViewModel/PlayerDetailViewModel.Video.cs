// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Pgc;
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
        await InitializeOriginalVideoSourceAsync();

        var view = _viewData as VideoPlayerView;

        if (!_useMpvPlayer)
        {
            SubtitleViewModel.SetData(view.Information.Identifier.Id, _currentPart.Id);
            DanmakuViewModel.SetData(view.Information.Identifier.Id, _currentPart.Id, _videoType);
        }

        DownloadViewModel.SetData("av" + view.Information.Identifier.Id, view.SubVideos);
    }

    private void CheckVideoHistory()
    {
        var view = _viewData as VideoPlayerView;
        if (view.Progress != null && view.Progress.Status == PlayedProgressStatus.Playing)
        {
            var history = view.Progress.Identifier;

            var ts = TimeSpan.FromSeconds(view.Progress.Progress);

            var needNotify = !SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoLoadHistoryWhenLoaded, true);
            if (needNotify)
            {
                IsShowProgressTip = true;
                ProgressTip = $"{ResourceToolkit.GetLocalizedString(StringNames.PreviousView)}{history.Title} {ts}";
            }
            else
            {
                JumpToLastProgressCommand.Execute(default);
            }
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
            if (IsInteractionVideo && !_useMpvPlayer)
            {
                InteractionViewModel.SetData(view.Information.Identifier.Id, default, view.InteractionVideo.GraphVersion);
            }
        }
    }

    private async Task InitializeVideoMediaInformationAsync()
    {
        var view = _viewData as VideoPlayerView;
        _mediaInformation = await PlayerProvider.GetVideoMediaInformationAsync(view.Information.Identifier.Id, _currentPart.Id);
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
        IsPosterShown = true;
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

            // 使用响应里有效的 URL
#pragma warning disable SA1010
            string[] videoUrls = [_video.BaseUrl, .._video.BackupUrls];
#pragma warning restore SA1010

            if (SettingsToolkit.ReadLocalSetting(SettingNames.NoP2P, false))
            {
                videoUrls = videoUrls.Where(url => !IsP2PUrl(url)).ToArray();
            }

            _video.BaseUrl = await PlayerProvider.GetAvailableUrlAsync(videoUrls);

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

#pragma warning disable SA1010
            string[] audioUrls = [_audio.BaseUrl, .._audio.BackupUrls];
#pragma warning restore SA1010
            if (SettingsToolkit.ReadLocalSetting(SettingNames.NoP2P, false))
            {
                audioUrls = audioUrls.Where(url => !IsP2PUrl(url)).ToArray();
            }

            // 使用响应里有效的 URL
            _audio.BaseUrl = await PlayerProvider.GetAvailableUrlAsync(audioUrls);
        }

        if (_video == null)
        {
            IsError = true;
            ErrorText = ResourceToolkit.GetLocalizedString(StringNames.SourceNotSupported);
            return;
        }

        try
        {
            if (_useMpvPlayer)
            {
                await PlayWithMpvAsync(_video, _audio, IsAudioOnly);
            }
            else
            {
                await Player.SetSourceAsync(_video, _audio, IsAudioOnly);
                StartTimers();
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorText = ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFailed);
            LogException(ex);
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

    private async Task PlayWithMpvAsync(SegmentInformation video, SegmentInformation audio, bool audioOnly)
    {
        var httpParams = $"--cookies --user-agent=\"{ServiceConstants.DefaultUserAgentString}\" --http-header-fields=\"Cookie: {AuthorizeProvider.GetCookieString()}\" --http-header-fields=\"Referer: https://www.bilibili.com\"";
        var videoUrl = video.BaseUrl;
        var audioUrl = audio.BaseUrl;
        var title = string.Empty;
        var progress = TimeSpan.FromSeconds(ProgressSeconds);
        var command = $"mpv {httpParams}";
        if (_viewData is VideoPlayerView videoView)
        {
            title = videoView.Information.Identifier.Title;
            if (videoView.Progress != null && videoView.Progress.Status == PlayedProgressStatus.Playing)
            {
                progress = TimeSpan.FromSeconds(videoView.Progress.Progress);
            }

            command += $" --script-opts=\"cid={_currentPart.Id}\"";
        }
        else if (_viewData is PgcPlayerView pgcView)
        {
            title = pgcView.Information.Identifier.Title;
            if (pgcView.Progress != null && pgcView.Progress.Status == PlayedProgressStatus.Playing)
            {
                progress = TimeSpan.FromSeconds(pgcView.Progress.Progress);
            }

            command += $" --script-opts=\"cid={_currentEpisode.PartId}\"";
        }

        if (!string.IsNullOrEmpty(title))
        {
            command += $" --title=\"{title}\"";
        }

        if (progress != TimeSpan.Zero)
        {
            command += $" --start=\"{progress.ToString(@"hh\:mm\:ss")}\"";
        }

        if (!string.IsNullOrEmpty(audioUrl) && audioOnly)
        {
            command += $" \"{audioUrl}\"";
        }
        else if (!audioOnly)
        {
            command += $" \"{videoUrl}\"";

            if (!string.IsNullOrEmpty(audioUrl))
            {
                command += $" --audio-file=\"{audioUrl}\"";
            }
        }

        try
        {
            ResetMpvPlayer();

            await Task.Run(() =>
            {
                _mpvProcess = new Process();
                _mpvProcess.StartInfo.FileName = "mpv";
                _mpvProcess.StartInfo.Arguments = command;
                _mpvProcess.StartInfo.UseShellExecute = false;
                _mpvProcess.StartInfo.RedirectStandardOutput = true;
                _mpvProcess.StartInfo.RedirectStandardError = true;
                _mpvProcess.StartInfo.RedirectStandardInput = true;
                _mpvProcess.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                _mpvProcess.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
                _mpvProcess.StartInfo.CreateNoWindow = true;
                _mpvProcess.OutputDataReceived += OnMpvProcessDataReceived;
                _mpvProcess.ErrorDataReceived += OnMpvProcessDataReceived;
                _mpvProcess.Start();
                _mpvProcess.BeginErrorReadLine();
                _mpvProcess.BeginOutputReadLine();
            });
        }
        catch (Exception)
        {
        }
    }

    private void OnMpvProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        var msg = e.Data;
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }

        if (msg.Contains("V:") || msg.Contains("A:"))
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                if (msg.Contains("Paused"))
                {
                    Status = PlayerStatus.Pause;
                }
                else if (msg.Contains("Buffering"))
                {
                    Status = PlayerStatus.Buffering;
                }
                else
                {
                    Status = PlayerStatus.Playing;
                }
            });

            ParsePositionAndDuration();
        }
        else
        {
            if (_mpvDebugMessages.Count > 0 && _mpvDebugMessages.Last() == msg)
            {
                return;
            }

            if (_mpvDebugMessages.Count >= 5)
            {
                _mpvDebugMessages.RemoveAt(0);
            }

            _mpvDebugMessages.Add(msg);
            _dispatcherQueue.TryEnqueue(() =>
            {
                MpvDebugString = string.Join('\n', _mpvDebugMessages);
            });
        }

        bool ParsePositionAndDuration()
        {
            var position = TimeSpan.Zero;
            var duration = TimeSpan.Zero;
            var pattern = @"AV:\s*(\d{2}:\d{2}:\d{2}\.\d{3})\s*/\s*(\d{2}:\d{2}:\d{2}\.\d{3})";

            var match = Regex.Match(msg, pattern);
            if (match.Success)
            {
                position = TimeSpan.Parse(match.Groups[1].Value);
                duration = TimeSpan.Parse(match.Groups[2].Value);

                _dispatcherQueue.TryEnqueue(() =>
                {
                    var args = new MediaPositionChangedEventArgs(position, duration);
                    OnMediaPositionChanged(default, args);
                });
            }

            return match.Success;
        }
    }
}
