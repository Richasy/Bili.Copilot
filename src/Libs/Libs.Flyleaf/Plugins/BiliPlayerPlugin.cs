// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Flyleaf.Plugins;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Data.Player;

namespace Bili.Copilot.Libs.Flyleaf.MediaFramework.MediaPlaylist;

/// <summary>
/// 哔哩播放器插件.
/// </summary>
public class BiliPlayerPlugin : PluginBase, IOpen, ISuggestExternalAudio, ISuggestExternalVideo
{
    private PlaylistItem _videoItem;

    /// <inheritdoc/>
    public new int Priority => 1;

    /// <inheritdoc/>
    public bool CanOpen()
    {
        if (Playlist.IOStream != null)
        {
            return false;
        }

        _videoItem = Playlist.Selected;
        return _videoItem != null;
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
            if (_videoItem != null)
            {
                var hasVideo = _videoItem.Tag.TryGetValue("video", out var videoObj);
                var hasAudio = _videoItem.Tag.TryGetValue("audio", out var audioObj);
                var hasCookie = _videoItem.Tag.TryGetValue("cookie", out var cookieObj);

                var videoData = videoObj as SegmentInformation;
                var audioData = audioObj as SegmentInformation;

                var headers = new Dictionary<string, string>();
                if (hasCookie)
                {
                    headers.Add("Cookie", cookieObj.ToString());
                }

                if (hasVideo)
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

                    AddExternalStream(videoStream, videoData, _videoItem);
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

                    AddExternalStream(audioStream, audioData, _videoItem);
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
}
