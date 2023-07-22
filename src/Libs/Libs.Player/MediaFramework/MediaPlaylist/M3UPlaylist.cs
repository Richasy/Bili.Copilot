// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

/// <summary>
/// M3U 播放列表项.
/// </summary>
public class M3UPlaylistItem
{
    /// <summary>
    /// 持续时间.
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 原始标题.
    /// </summary>
    public string OriginalTitle { get; set; }

    /// <summary>
    /// URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 用户代理.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// 引用页.
    /// </summary>
    public string Referrer { get; set; }

    /// <summary>
    /// 地理位置屏蔽.
    /// </summary>
    public bool GeoBlocked { get; set; }

    /// <summary>
    /// 非 24/7.
    /// </summary>
    public bool Not_24_7 { get; set; }

    /// <summary>
    /// 高度.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 标签字典.
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// 表示一个 M3U 播放列表.
/// </summary>
public static class M3UPlaylist
{
    /// <summary>
    /// 从指定的 URL 下载并解析 M3U 播放列表.
    /// </summary>
    /// <param name="url">要下载的 URL.</param>
    /// <param name="timeoutMs">下载超时时间（以毫秒为单位），默认为 30000 毫秒.</param>
    /// <returns>解析后的 M3U 播放列表项列表.</returns>
    public static List<M3UPlaylistItem> ParseFromHttp(string url, int timeoutMs = 30000)
    {
        var downStr = Utils.DownloadToString(url, timeoutMs);
        if (downStr == null)
        {
            return null;
        }

        using StringReader reader = new(downStr);
        return Parse(reader);
    }

    /// <summary>
    /// 解析给定的 M3U 播放列表字符串.
    /// </summary>
    /// <param name="text">要解析的 M3U 播放列表字符串.</param>
    /// <returns>解析后的 M3U 播放列表项列表.</returns>
    public static List<M3UPlaylistItem> ParseFromString(string text)
    {
        using StringReader reader = new(text);
        return Parse(reader);
    }

    /// <summary>
    /// 解析指定文件中的 M3U 播放列表.
    /// </summary>
    /// <param name="filename">要解析的文件名.</param>
    /// <returns>解析后的 M3U 播放列表项列表.</returns>
    public static List<M3UPlaylistItem> Parse(string filename)
    {
        using StreamReader reader = new(filename);
        return Parse(reader);
    }

    /// <summary>
    /// 解析给定的文本读取器中的 M3U 播放列表.
    /// </summary>
    /// <param name="reader">要解析的文本读取器.</param>
    /// <returns>解析后的 M3U 播放列表项列表.</returns>
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
                    {
                        item.Tags.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }

                item.Title = GetMatch(line, @",\s*(.*)$");
                item.OriginalTitle = item.Title;

                if (item.Title.IndexOf(" [Geo-blocked]") >= 0)
                {
                    item.GeoBlocked = true;
                    item.Title = item.Title.Replace(" [Geo-blocked]", string.Empty);
                }

                if (item.Title.IndexOf(" [Not 24/7]") >= 0)
                {
                    item.Not_24_7 = true;
                    item.Title = item.Title.Replace(" [Not 24/7]", string.Empty);
                }

                var height = Regex.Match(item.Title, " \\(([0-9]+)p\\)");
                if (height.Groups.Count == 2)
                {
                    item.Height = int.Parse(height.Groups[1].Value);
                    item.Title = item.Title.Replace(height.Groups[0].Value, string.Empty);
                }

                while ((line = reader.ReadLine()) != null && line.StartsWith("#EXTVLCOPT"))
                {
                    if (item.UserAgent == null)
                    {
                        item.UserAgent = GetMatch(line, "http-user-agent\\s*=\\s*\"*(.*)\"*");
                        if (item.UserAgent != null)
                        {
                            continue;
                        }
                    }

                    if (item.Referrer == null)
                    {
                        item.Referrer = GetMatch(line, "http-referrer\\s*=\\s*\"*(.*)\"*");
                        if (item.Referrer != null)
                        {
                            continue;
                        }
                    }
                }

                item.Url = line;
                items.Add(item);
            }
        }

        return items;
    }

    /// <summary>
    /// 从给定的文本中获取匹配指定模式的第一个结果.
    /// </summary>
    /// <param name="text">要匹配的文本.</param>
    /// <param name="pattern">匹配模式.</param>
    /// <returns>匹配结果的第一个捕获组的值，如果没有匹配则返回 null.</returns>
    private static string GetMatch(string text, string pattern)
    {
        var match = Regex.Match(text, pattern);
        return match.Success && match.Groups.Count > 1 ? match.Groups[1].Value : null;
    }
}
