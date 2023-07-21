using System;
using System.IO;

using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

namespace Bili.Copilot.Libs.Player.Plugins;

public class OpenDefault : PluginBase, IOpen, IScrapeItem
{
    /* TODO
     * 
     * 1) Current Url Syntax issues
     *  ..\..\..\..\folder\file.mp3 | Cannot handle this
     *  file:///C:/folder/fi%20le.mp3 | FFmpeg & File.Exists cannot handle this
     * 
     */

    public new int Priority { get; set; } = 3000;

    public bool CanOpen() => true;

    public OpenResults Open()
    {
        try
        {
            if (Playlist.IOStream != null)
            {
                AddPlaylistItem(new PlaylistItem()
                {
                    IOStream = Playlist.IOStream,
                    Title = "Custom IO Stream",
                    FileSize = Playlist.IOStream.Length
                });

                Handler.OnPlaylistCompleted();

                return new OpenResults();
            }

            // Proper Url Format
            string scheme;
            bool isWeb = false;
            string uriType = "";
            string ext = Utils.GetUrlExtention(Playlist.Url);
            string localPath = null;

            try
            {
                Uri uri = new(Playlist.Url);
                scheme = uri.Scheme.ToLower();
                isWeb = scheme.StartsWith("http");
                uriType = uri.IsFile ? "file" : (uri.IsUnc ? "unc" : "");
                localPath = uri.LocalPath;
            }
            catch { }


            // Playlists (M3U, PLS | TODO: WPL, XSPF)
            if (ext == "m3u")
            {
                Playlist.InputType = InputType.Web; // TBR: Can be mixed
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
                        Referrer = mitem.Referrer
                    });
                }

                Handler.OnPlaylistCompleted();

                return new OpenResults();
            }
            else if (ext == "pls")
            {
                Playlist.InputType = InputType.Web; // TBR: Can be mixed
                Playlist.FolderBase = Path.GetTempPath();

                var items = PLSPlaylist.Parse(Playlist.Url);

                foreach (var mitem in items)
                {
                    AddPlaylistItem(new PlaylistItem()
                    {
                        Title = mitem.Title,
                        Url = mitem.Url,
                        DirectUrl = mitem.Url,
                        // Duration
                    });
                }

                Handler.OnPlaylistCompleted();

                return new OpenResults();
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
                //Playlist.InputType = InputType.Unknown;
                Playlist.FolderBase = Path.GetTempPath();
            }

            PlaylistItem item = new()
            {
                Url = Playlist.Url,
                DirectUrl = Playlist.Url
            };

            if (File.Exists(Playlist.Url))
            {
                FileInfo fi = new(Playlist.Url);
                item.Title = fi.Extension == null ? fi.Name : fi.Name[..^fi.Extension.Length];
                item.FileSize = fi.Length;
            }
            else
                item.Title = localPath != null ? Path.GetFileName(localPath) : Playlist.Url;

            AddPlaylistItem(item);
            Handler.OnPlaylistCompleted();

            return new OpenResults();
        }
        catch (Exception e)
        {
            return new OpenResults(e.Message);
        }
    }

    public OpenResults OpenItem() => new OpenResults();

    public void ScrapeItem(PlaylistItem item)
    {
        // Update Title (TBR: don't mess with other media types - only movies/tv shows)
        if (Playlist.InputType != InputType.File && Playlist.InputType != InputType.UNC && Playlist.InputType != InputType.Torrent)
            return;

        var mp = Utils.GetMediaParts(item.OriginalTitle);
        item.Title = mp.Title;

        if (mp.Season > 0)
        {
            item.Season = mp.Season;
            item.Episode = mp.Episode;
            item.Title += $" S{item.Season}E{item.Episode}";
        }
    }
}
