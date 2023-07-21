// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 打开字幕插件.
/// </summary>
public class OpenSubtitlePlugin : PluginBase, IOpenSubtitlePlugin, ISearchLocalSubtitlePlugin
{
    /// <inheritdoc/>
    public new int Priority { get; set; } = 3000;

    /// <inheritdoc/>
    public OpenSubtitleResult Open(string url)
    {
        foreach (var extStream in Selected.ExternalSubtitlesStreams)
        {
            if (extStream.Url == url || extStream.DirectUrl == url)
            {
                return new OpenSubtitleResult(extStream);
            }
        }

        string title;

        try
        {
            FileInfo fi = new(Playlist.Url);
            title = fi.Extension == null ? fi.Name : fi.Name[..^fi.Extension.Length];
        }
        catch
        {
            title = url;
        }

        ExternalSubtitleStream newExtStream = new()
        {
            Url = url,
            Title = title,
            Downloaded = true,
        };

        AddExternalStream(newExtStream);

        return new OpenSubtitleResult(newExtStream);
    }

    /// <inheritdoc/>
    public OpenSubtitleResult Open(Stream ioStream) => null;

    /// <inheritdoc/>
    public void SearchLocalSubtitle()
    {
        List<string> files = new();

        try
        {
            foreach (var lang in Config.Subtitles.Languages)
            {
                var prefix = Selected.Title[..Math.Min(Selected.Title.Length, 4)];
                var folder = Path.Combine(Playlist.FolderBase, Selected.Folder, "Subs");
                if (!Directory.Exists(folder))
                {
                    return;
                }

                var filesCur = Directory.GetFiles(Path.Combine(Playlist.FolderBase, Selected.Folder, "Subs"), $"{prefix}*{lang.IdSubLanguage}.utf8*.srt");
                foreach (var file in filesCur)
                {
                    var exists = false;
                    foreach (var extStream in Selected.ExternalSubtitlesStreams)
                    {
                        if (extStream.Url == file)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (exists)
                    {
                        continue;
                    }

                    var mp = Utils.GetMediaPart(file);
                    if (Selected.Season > 0 && (Selected.Season != mp.Season || Selected.Episode != mp.Episode))
                    {
                        continue;
                    }

                    Log.Debug($"Adding [{lang}] {file}");

                    AddExternalStream(new ExternalSubtitleStream()
                    {
                        Url = file,
                        Title = file,
                        Converted = true,
                        Downloaded = true,
                        Language = lang,
                    });
                }
            }
        }
        catch
        {
        }
    }
}
