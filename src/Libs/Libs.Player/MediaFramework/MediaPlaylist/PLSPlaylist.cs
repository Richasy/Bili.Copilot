// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

/// <summary>
/// 表示一个 PLS 播放列表.
/// </summary>
public static class PlsPlaylist
{
    /// <summary>
    /// 解析 PLS 播放列表文件.
    /// </summary>
    /// <param name="filename">要解析的文件名.</param>
    /// <returns>解析得到的播放列表项列表.</returns>
    public static List<PlsPlaylistItem> Parse(string filename)
    {
        List<PlsPlaylistItem> items = new();

        string res;
        var entries = 1000;

        if ((res = GetINIAttribute("playlist", "NumberOfEntries", filename)) != null)
        {
            entries = int.Parse(res);
        }

        for (var i = 1; i <= entries; i++)
        {
            if ((res = GetINIAttribute("playlist", $"File{i}", filename)) == null)
            {
                break;
            }

            PlsPlaylistItem item = new() { Url = res };

            if ((res = GetINIAttribute("playlist", $"Title{i}", filename)) != null)
            {
                item.Title = res;
            }

            if ((res = GetINIAttribute("playlist", $"Length{i}", filename)) != null)
            {
                item.Duration = int.Parse(res);
            }

            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// 从 INI 文件中获取属性值.
    /// </summary>
    /// <param name="name">节的名称.</param>
    /// <param name="key">键的名称.</param>
    /// <param name="path">INI 文件的路径.</param>
    /// <returns>获取到的属性值.</returns>
    public static string GetINIAttribute(string name, string key, string path)
    {
        StringBuilder sb = new(255);
        return GetPrivateProfileString(name, key, string.Empty, sb, 255, path) > 0
            ? sb.ToString() : null;
    }

    /// <summary>
    /// 写入私有配置文件的字符串.
    /// </summary>
    /// <param name="name">节的名称.</param>
    /// <param name="key">键的名称.</param>
    /// <param name="val">要写入的值.</param>
    /// <param name="filePath">配置文件的路径.</param>
    /// <returns>操作是否成功.</returns>
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string name, string key, string val, string filePath);

    /// <summary>
    /// 从私有配置文件中获取字符串.
    /// </summary>
    /// <param name="section">节的名称.</param>
    /// <param name="key">键的名称.</param>
    /// <param name="def">默认值.</param>
    /// <param name="retVal">用于存储返回值的 StringBuilder 对象.</param>
    /// <param name="size">StringBuilder 对象的大小.</param>
    /// <param name="filePath">配置文件的路径.</param>
    /// <returns>获取到的字符串.</returns>
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
}

/// <summary>
/// 表示一个播放列表项.
/// </summary>
public class PlsPlaylistItem
{
    /// <summary>
    /// 获取或设置播放时长（以秒为单位）.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 获取或设置标题.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 获取或设置 URL.
    /// </summary>
    public string Url { get; set; }
}
