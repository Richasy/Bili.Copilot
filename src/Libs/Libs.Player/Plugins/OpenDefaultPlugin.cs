// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 默认的启动插件.
/// </summary>
public class OpenDefaultPlugin : PluginBase, IOpenPlugin, IScrapeItemPlugin
{
    /// <inheritdoc/>
    public new int Priority { get; set; } = 3000;

    /// <inheritdoc/>
    public bool CanOpen() => true;

    /// <inheritdoc/>
    public OpenResult Open()
    {
        try
        {
            if (Playlist.IOStream != null)
            {
                AddPlaylistItem(new PlaylistItem()
                {
                    IoStream = Playlist.IOStream,
                    Title = "Custom IO Stream",
                    FileSize = Playlist.IOStream.Length,
                });

                Handler.OnPlaylistCompleted();

                return new OpenResult();
            }

            // Proper Url Format
            string scheme;
            var isWeb = false;
            var uriType = string.Empty;
            var ext = Utils.GetUrlExtension(Playlist.Url);
            string localPath = null;

            try
            {
                Uri uri = new(Playlist.Url);
                scheme = uri.Scheme.ToLower();
                isWeb = scheme.StartsWith("http");
                uriType = uri.IsFile ? "file" : (uri.IsUnc ? "unc" : string.Empty);
                localPath = uri.LocalPath;
            }
            catch
            {
            }

            // Playlists (M3U, PLS | TODO: WPL, XSPF)
            if (ext == "m3u")
            {
                Playlist.InputType = InputType.Web;
                Playlist.FolderBase = Path.GetTempPath();

                var items = isWeb ? M3UPlaylist.ParseFromHttp(Playlist.Url) : M3UPlaylist.Parse(Playlist.Url);

                foreach (var mitem in items)
                {
                    AddPlaylistItem(new PlaylistItem()
                    {
                        Title = mitem.Title,
                        Url = mitem.Url,
                        DirectUrl = mitem.Url,
                        UserAgent = mitem.UserAgent,
                        Referrer = mitem.Referrer,
                    });
                }

                Handler.OnPlaylistCompleted();

                return new OpenResult();
            }
            else if (ext == "pls")
            {
                Playlist.InputType = InputType.Web; // TBR: Can be mixed
                Playlist.FolderBase = Path.GetTempPath();

                var items = PlsPlaylist.Parse(Playlist.Url);

                foreach (var mitem in items)
                {
                    AddPlaylistItem(new PlaylistItem()
                    {
                        Title = mitem.Title,
                        Url = mitem.Url,
                        DirectUrl = mitem.Url,
                    });
                }

                Handler.OnPlaylistCompleted();

                return new OpenResult();
            }

            // Single Playlist Item
            if (uriType == "file")
            {
                if (ext == "mpd")
                {
                    Playlist.InputType = InputType.Web;
                    Playlist.FolderBase = Path.GetTempPath();
                    AddPlaylistItem(new PlaylistItem()
                    {
                        Title = "MPD Playlist",
                        Referrer = "https://dash.akamaized.net",
                    });
                }
                else
                {
                    Playlist.InputType = InputType.File;
                    if (File.Exists(Playlist.Url))
                    {
                        FileInfo fi = new(Playlist.Url);
                        Playlist.FolderBase = fi.DirectoryName;
                    }
                }
            }
            else if (isWeb)
            {
                Playlist.InputType = InputType.Web;
                Playlist.FolderBase = Path.GetTempPath();
            }
            else if (uriType == "unc")
            {
                Playlist.InputType = InputType.UNC;
                Playlist.FolderBase = Path.GetTempPath();
            }
            else
            {
                Playlist.FolderBase = Path.GetTempPath();
            }

            PlaylistItem item = new()
            {
                Url = Playlist.Url,
                DirectUrl = Playlist.Url,
            };

            if (File.Exists(Playlist.Url))
            {
                FileInfo fi = new(Playlist.Url);
                item.Title = fi.Extension == null ? fi.Name : fi.Name[..^fi.Extension.Length];
                item.FileSize = fi.Length;
            }
            else
            {
                item.Title = localPath != null ? Path.GetFileName(localPath) : Playlist.Url;
            }

            AddPlaylistItem(item);
            Handler.OnPlaylistCompleted();

            return new OpenResult();
        }
        catch (Exception e)
        {
            return new OpenResult(e.Message);
        }
    }

    /// <inheritdoc/>
    public OpenResult OpenItem() => new();

    /// <inheritdoc/>
    public void ScrapeItem(PlaylistItem item)
    {
        // Update Title (TBR: don't mess with other media types - only movies/tv shows)
        if (Playlist.InputType != InputType.File && Playlist.InputType != InputType.UNC && Playlist.InputType != InputType.Torrent)
        {
            return;
        }

        var mp = Utils.GetMediaPart(item.OriginalTitle);
        item.Title = mp.Title;

        if (mp.Season > 0)
        {
            item.Season = mp.Season;
            item.Episode = mp.Episode;
            item.Title += $" S{item.Season}E{item.Episode}";
        }
    }
}
