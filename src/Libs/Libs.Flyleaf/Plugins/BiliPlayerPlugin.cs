﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Text;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Data.Player;
using FlyleafLib.MediaFramework.MediaPlaylist;
using FlyleafLib.MediaFramework.MediaStream;

namespace FlyleafLib.Plugins;

/// <summary>
/// 哔哩播放器插件.
/// </summary>
public class BiliPlayerPlugin : PluginBase, IOpen, ISuggestExternalAudio, ISuggestExternalVideo
{
    private PlaylistItem _playItem;

    /// <inheritdoc/>
    public new int Priority => 1;

    /// <inheritdoc/>
    public bool CanOpen()
    {
        if (Playlist.IOStream != null)
        {
            return false;
        }

        _playItem = Playlist.Selected;
        return _playItem != null;
    }

    /// <inheritdoc/>
    public OpenResults Open()
        => new();

    /// <inheritdoc/>
    public OpenResults OpenItem()
    {
        Playlist.InputType = InputType.Web;
        try
        {
            if (_playItem != null)
            {
                var hasVideo = _playItem.Tag.TryGetValue("video", out var videoObj);
                var hasAudio = _playItem.Tag.TryGetValue("audio", out var audioObj);
                var hasLive = _playItem.Tag.TryGetValue("live", out var liveObj);
                var hasCookie = _playItem.Tag.TryGetValue("cookie", out var cookieObj);
                var hasOnlyAudio = _playItem.Tag.TryGetValue("onlyAudio", out var onlyAudioObj);
                var hasWebDav = _playItem.Tag.TryGetValue("webdav", out var webDavObj);

                var videoData = videoObj as SegmentInformation;
                var audioData = audioObj as SegmentInformation;
                var liveData = liveObj as string;
                var webDavData = webDavObj as WebDavVideoInformation;
                var onlyAudio = hasOnlyAudio && (bool)onlyAudioObj;

                var headers = new Dictionary<string, string>();
                if (hasCookie)
                {
                    headers.Add("Cookie", cookieObj.ToString());
                }

                if (hasVideo && !onlyAudio)
                {
                    var videoStream = new ExternalVideoStream()
                    {
                        Url = videoData.BaseUrl,
                        UrlFallback = videoData.BackupUrls?.FirstOrDefault(),
                        HasAudio = hasAudio,
                        Codec = videoData.Codecs,
                        Width = videoData.Width,
                        Height = videoData.Height,
                        UserAgent = ServiceConstants.DefaultUserAgentString,
                        Referrer = "https://www.bilibili.com",
                        HTTPHeaders = headers,
                    };

                    AddExternalStream(videoStream, videoData, _playItem);
                }

                if (hasAudio)
                {
                    var audioStream = new ExternalAudioStream()
                    {
                        Url = audioData.BaseUrl,
                        UrlFallback = audioData.BackupUrls?.FirstOrDefault(),
                        Codec = videoData.Codecs,
                        UserAgent = ServiceConstants.DefaultUserAgentString,
                        Referrer = "https://www.bilibili.com",
                        HTTPHeaders = headers,
                    };

                    AddExternalStream(audioStream, audioData, _playItem);
                }

                if (hasLive)
                {
                    headers.Add("Origin", "https://live.bilibili.com");
                    headers.Add("User-Agent", "Mozilla/5.0 BiliDroid/1.12.0 (bbcallen@gmail.com)");

                    ExternalStream externalStream = onlyAudio
                       ? new ExternalAudioStream()
                       {
                           Url = liveData,
                           Referrer = "https://live.bilibili.com",
                           HTTPHeaders = headers,
                       }
                       : new ExternalVideoStream()
                       {
                           Url = liveData,
                           Referrer = "https://live.bilibili.com",
                           HTTPHeaders = headers,
                           HasAudio = true,
                       };

                    AddExternalStream(externalStream, liveData, _playItem);
                }

                if (hasWebDav)
                {
                    var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{webDavData.Config.UserName}:{webDavData.Config.Password}"));
                    headers.Add("Authorization", $"Basic {token}");
                    var externalStream = new ExternalVideoStream()
                    {
                        Url = AppToolkit.GetWebDavServer(webDavData.Config.Host, webDavData.Config.Port) + webDavData.Href,
                        HTTPHeaders = headers,
                        HasAudio = true,
                    };

                    AddExternalStream(externalStream, webDavData.Href, _playItem);
                }
            }
        }
        catch (System.Exception e)
        {
            Log.Error($"Open ({e.Message})");
            return new OpenResults(e.Message);
        }

        return new();
    }

    /// <inheritdoc/>
    public ExternalAudioStream SuggestExternalAudio()
        => Selected.ExternalAudioStreams.FirstOrDefault();

    /// <inheritdoc/>
    public ExternalVideoStream SuggestExternalVideo()
        => Selected.ExternalVideoStreams.FirstOrDefault();

    /// <inheritdoc/>
    public override void Dispose()
    {
        _playItem = null;
    }
}
