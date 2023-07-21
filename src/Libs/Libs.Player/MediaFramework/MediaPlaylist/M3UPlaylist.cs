﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

public class M3UPlaylistItem
{
    public long     Duration    { get; set; }
    public string   Title       { get; set; }
    public string   OriginalTitle
                                { get; set; }
    public string   Url         { get; set; }
    public string   UserAgent   { get; set; }
    public string   Referrer    { get; set; }
    public bool     GeoBlocked  { get; set; }
    public bool     Not_24_7    { get; set; }
    public int      Height      { get; set; }

    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
}

public class M3UPlaylist
{
    public static List<M3UPlaylistItem> ParseFromHttp(string url, int timeoutMs = 30000)
    {
        string downStr = Utils.DownloadToString(url, timeoutMs);
        if (downStr == null)
            return null;

        using StringReader reader = new(downStr);
        return Parse(reader);
    }

    public static List<M3UPlaylistItem> ParseFromString(string text)
    {
        using StringReader reader = new(text);
        return Parse(reader);
    }

    public static List<M3UPlaylistItem> Parse(string filename)
    {
        using StreamReader reader = new(filename);
        return Parse(reader);
    }
    private static List<M3UPlaylistItem> Parse(TextReader reader)
    {
        string line;
        List<M3UPlaylistItem> items = new();

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("#EXTINF"))
            {
                M3UPlaylistItem item = new();
                var matches = Regex.Matches(line, " ([^\\s=]+)=\"([^\\s\"]+)\"");
                foreach (Match match in matches)
                {
                    if (match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value))
                        item.Tags.Add(match.Groups[1].Value, match.Groups[2].Value);
                }

                item.Title = GetMatch(line, @",\s*(.*)$");
                item.OriginalTitle = item.Title;

                if (item.Title.IndexOf(" [Geo-blocked]") >= 0)
                {
                    item.GeoBlocked = true;
                    item.Title = item.Title.Replace(" [Geo-blocked]", "");
                }

                if (item.Title.IndexOf(" [Not 24/7]") >= 0)
                {
                    item.Not_24_7 = true;
                    item.Title = item.Title.Replace(" [Not 24/7]", "");
                }

                var height = Regex.Match(item.Title, " \\(([0-9]+)p\\)");
                if (height.Groups.Count == 2)
                {
                    item.Height = int.Parse(height.Groups[1].Value);
                    item.Title = item.Title.Replace(height.Groups[0].Value, "");
                }

                while ((line = reader.ReadLine()) != null && line.StartsWith("#EXTVLCOPT"))
                {
                    if (item.UserAgent == null)
                    {
                        item.UserAgent = GetMatch(line, "http-user-agent\\s*=\\s*\"*(.*)\"*");
                        if (item.UserAgent != null) continue;
                    }

                    if (item.Referrer == null)
                    {
                        item.Referrer = GetMatch(line, "http-referrer\\s*=\\s*\"*(.*)\"*");
                        if (item.Referrer != null) continue;
                    }
                }

                item.Url = line;
                items.Add(item);
            }
        }

        return items;
    }

    private static string GetMatch(string text, string pattern)
    {
        var match = Regex.Match(text, pattern);
        return match.Success && match.Groups.Count > 1 ? match.Groups[1].Value : null;
    }
}
