// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.IO;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.Misc;
using CommunityToolkit.Mvvm.ComponentModel;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

/// <summary>
/// 播放列表类.
/// </summary>
public partial class Playlist : ObservableObject
{
    private readonly object _lockItems = new();

    [ObservableProperty]
    private int _expectingItems;

    /// <summary>
    /// 播放列表的标题.
    /// </summary>
    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 播放列表的打开/选中项.
    /// </summary>
    [ObservableProperty]
    private PlaylistItem _selected;

    private string _url;
    private long _openCounter;

    /// <summary>
    /// 构造函数，初始化播放列表对象.
    /// </summary>
    /// <param name="uniqueId">唯一标识符.</param>
    public Playlist(int uniqueId)
        => Log = new LogHandler(("[#" + uniqueId + "]").PadRight(8, ' ') + " [Playlist] ");

    /// <summary>
    /// 用户提供的 URL.
    /// </summary>
    public string Url
    {
        get => _url; set
        {
            var fixedUrl = FixFileUrl(value);
            SetProperty(ref _url, fixedUrl);
        }
    }

    /// <summary>
    /// 用户提供的 IO 流.
    /// </summary>
    public Stream IOStream { get; set; }

    /// <summary>
    /// 播放列表的文件夹基础路径，可用于保存相关文件.
    /// </summary>
    public string FolderBase { get; set; }

    /// <summary>
    /// 播放列表是否已完成.
    /// </summary>
    public bool Completed { get; set; }

    /// <summary>
    /// 提供的输入类型（如文件、UNC、种子、网络等）.
    /// </summary>
    public InputType InputType { get; set; }

    /// <summary>
    /// 播放列表的项集合.
    /// </summary>
    public ObservableCollection<PlaylistItem> Items { get; set; } = new ObservableCollection<PlaylistItem>();

    internal DecoderContext Decoder { get; set; }

    internal LogHandler Log { get; }

    /// <summary>
    /// 重置播放列表.
    /// </summary>
    public void Reset()
    {
        _openCounter = Decoder.OpenCounter;

        lock (_lockItems)
        {
            Items.Clear();
        }

        var noupdate = _url == null && Title == null && Selected == null;

        _url = null;
        Title = null;
        Selected = null;
        IOStream = null;
        FolderBase = null;
        Completed = false;
        ExpectingItems = 0;

        InputType = InputType.Unknown;

        if (!noupdate)
        {
            UI(() =>
            {
                OnPropertyChanged(nameof(Url));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Selected));
            });
        }
    }

    /// <summary>
    /// 向播放列表中添加项.
    /// </summary>
    /// <param name="item">要添加的项.</param>
    /// <param name="pluginName">插件名称.</param>
    /// <param name="tag">标签对象.</param>
    public void AddItem(PlaylistItem item, string pluginName, object tag = null)
    {
        if (_openCounter != Decoder.OpenCounter)
        {
            Log.Debug("AddItem Cancelled");
            return;
        }

        lock (_lockItems)
        {
            Items.Add(item);
            Items[^1].Index = Items.Count - 1;

            if (tag != null)
            {
                item.AddTag(tag, pluginName);
            }
        }

        Decoder.ScrapeItem(item);
    }
}
