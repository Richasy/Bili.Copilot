using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using static FFmpeg.AutoGen.AVMediaType;
using static FFmpeg.AutoGen.ffmpeg;

using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.Plugins;

public unsafe class StreamSuggester : PluginBase, ISuggestPlaylistItem, ISuggestAudioStream, ISuggestVideoStream, ISuggestSubtitlesStream, ISuggestSubtitles, ISuggestBestExternalSubtitles
{
    public new int Priority { get; set; } = 3000;

    public AudioStream SuggestAudio(ObservableCollection<AudioStream> streams)
    {
        lock (streams[0].Demuxer.lockActions)
        {
            foreach (var lang in Config.Audio.Languages)
                foreach (var stream in streams)
                    if (stream.Language == lang)
                    {
                        if (stream.Demuxer.Programs.Count < 2)
                        {
                            Log.Info($"Audio based on language");
                            return stream;
                        }

                        for (int i = 0; i < stream.Demuxer.Programs.Count; i++)
                        {
                            bool aExists = false, vExists = false;
                            foreach (var pstream in stream.Demuxer.Programs[i].Streams)
                            {
                                if (pstream.StreamIndex == stream.StreamIndex) aExists = true;
                                else if (pstream.StreamIndex == stream.Demuxer.VideoStream?.StreamIndex) vExists = true;
                            }

                            if (aExists && vExists)
                            {
                                Log.Info($"Audio based on language and same program #{i}");
                                return stream;
                            }
                        }
                    }

            // Fall-back to FFmpeg's default
            int streamIndex;
            lock (streams[0].Demuxer.lockFmtCtx)
                streamIndex = av_find_best_stream(streams[0].Demuxer.FormatContext, AVMEDIA_TYPE_AUDIO, -1, streams[0].Demuxer.VideoStream != null ? streams[0].Demuxer.VideoStream.StreamIndex : -1, null, 0);

            foreach (var stream in streams)
                if (stream.StreamIndex == streamIndex)
                {
                    Log.Info($"Audio based on av_find_best_stream");
                    return stream;
                }

            if (streams.Count > 0) // FFmpeg will not suggest anything when findstreaminfo is disable
                return streams[0];

            return null;
        }
    }

    public VideoStream SuggestVideo(ObservableCollection<VideoStream> streams)
    {
        // Try to find best video stream based on current screen resolution
        var iresults =
            from vstream in streams
            where vstream.Type == MediaType.Video && vstream.Height <= Config.Video.MaxVerticalResolution //Decoder.VideoDecoder.Renderer.Info.ScreenBounds.Height
            orderby vstream.Height descending
            select vstream;

        List<VideoStream> results = iresults.ToList();

        if (results.Count != 0)
            return iresults.ToList()[0];
        else
        {
            // Fall-back to FFmpeg's default
            int streamIndex;
            lock (streams[0].Demuxer.lockFmtCtx)
                streamIndex = av_find_best_stream(streams[0].Demuxer.FormatContext, AVMEDIA_TYPE_VIDEO, -1, -1, null, 0);
            if (streamIndex < 0) return null;

            foreach (var vstream in streams)
                if (vstream.StreamIndex == streamIndex)
                    return vstream;
        }

        if (streams.Count > 0) // FFmpeg will not suggest anything when findstreaminfo is disable
            return streams[0];

        return null;
    }

    public PlaylistItem SuggestItem()
        => Playlist.Items[0];

    public void SuggestSubtitles(out SubtitlesStream stream, out ExternalSubtitlesStream extStream)
    {
        stream = null;
        extStream = null;

        List<Language> langs = new();

        foreach (var lang in Config.Subtitles.Languages)
            langs.Add(lang);

        langs.Add(Language.Unknown);

        var extStreams = Selected.ExternalSubtitlesStreams.OrderBy(x => x.Language.ToString()).ThenByDescending(x => x.Rating).ThenBy(x => x.Downloaded);

        foreach (var lang in langs)
        {
            foreach(var embStream in decoder.VideoDemuxer.SubtitlesStreams)
                if (embStream.Language == lang)
                {
                    stream = embStream;
                    return;
                }

            foreach(var extStream2 in extStreams)
                if (extStream2.Language == lang)
                {
                    extStream = extStream2;
                    return;
                }
        }
    }

    public ExternalSubtitlesStream SuggestBestExternalSubtitles()
    {
        var extStreams = Selected.ExternalSubtitlesStreams.OrderBy(x => x.Language.ToString()).ThenByDescending(x => x.Rating).ThenBy(x => x.Downloaded);

        foreach(var extStream in extStreams)
            if (extStream.Language == Config.Subtitles.Languages[0])
                return extStream;

        return null;
    }

    public SubtitlesStream SuggestSubtitles(ObservableCollection<SubtitlesStream> streams, List<Language> langs)
    {
        foreach(var lang in langs)
            foreach(var stream in streams)
                if (lang == stream.Language)
                    return stream;

        return null;
    }
}