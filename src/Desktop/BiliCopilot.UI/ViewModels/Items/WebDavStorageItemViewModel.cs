// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.ViewModels;
using WebDav;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// WebDav 存储项视图模型.
/// </summary>
public sealed partial class WebDavStorageItemViewModel : ViewModelBase<WebDavResource>
{
    [ObservableProperty]
    private bool _isFolder;

    [ObservableProperty]
    private string _icon;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavStorageItemViewModel"/> class.
    /// </summary>
    public WebDavStorageItemViewModel(WebDavResource data)
        : base(data)
    {
        IsFolder = data.IsCollection;

        var extension = Path.GetExtension(data.Uri).ToLower();
        if (IsFolder)
        {
            FileType = WebDavFileType.Directory;
            Icon = "📁";
        }
        else if (FileExtensions.Text.Contains(extension))
        {
            FileType = WebDavFileType.Text;
            Icon = "📑";
        }
        else if (FileExtensions.Video.Contains(extension))
        {
            FileType = WebDavFileType.Video;
            Icon = "🎞";
        }
        else if (FileExtensions.Audio.Contains(extension))
        {
            FileType = WebDavFileType.Audio;
            Icon = "🎧";
        }
        else if (FileExtensions.Image.Contains(extension))
        {
            FileType = WebDavFileType.Image;
            Icon = "🖼";
        }
        else
        {
            FileType = WebDavFileType.Other;
            Icon = "❔";
        }
    }

    /// <summary>
    /// 文件类型.
    /// </summary>
    public WebDavFileType FileType { get; }

    /// <summary>
    /// 是否可用.
    /// </summary>
    public bool IsEnabled => FileType == WebDavFileType.Directory || FileType == WebDavFileType.Video || FileType == WebDavFileType.Audio;

    [RelayCommand]
    private void Activate()
    {
        var pageVM = this.Get<WebDavPageViewModel>();
        if (IsFolder)
        {
            pageVM.LoadPathCommand.Execute(Data.Uri);
        }
        else
        {
            var videoList = pageVM.Items.Where(p => p.FileType == WebDavFileType.Video).Select(p => p.Data).ToList() ?? new List<WebDavResource>();
            this.Get<NavigationViewModel>().NavigateToOver(typeof(WebDavPlayerPage), (videoList, Data));
        }
    }

    internal static class FileExtensions
    {
        internal static readonly string[] Text = { ".txt", ".doc", ".docx", ".pdf", ".rtf", ".odt", ".csv", ".xml", ".html", ".json", ".srt", ".ass" };

        internal static readonly string[] Video = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".f4v", ".m4v", ".rmvb", ".rm", ".3gp", ".dat", ".mts", ".vob", ".mkv", ".mpeg", ".mpg", ".ogv", ".webm", ".asf", ".divx", ".qt", ".m2ts", ".ts" };

        internal static readonly string[] Audio = { ".mp3", ".wav", ".wma", ".ogg", ".aac", ".flac", ".ape", ".m4a", ".m4r", ".mid", ".ra", ".amr", ".aiff", ".au", ".raw", ".mp2", ".m3u", ".oga", ".opus", ".spx", ".alac", ".dts", ".ac3", ".aax", ".mka" };

        internal static readonly string[] Image = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".psd", ".svg", ".raw", ".ico", ".heif", ".indd", ".ai", ".eps", ".pdf", ".pict", ".tif", ".cr2", ".nef", ".orf", ".sr2" };
    }
}
