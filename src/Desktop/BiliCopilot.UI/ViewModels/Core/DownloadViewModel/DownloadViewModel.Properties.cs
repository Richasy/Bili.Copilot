// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 下载视图模型.
/// </summary>
public sealed partial class DownloadViewModel
{
    private const string VideoUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69";
    private readonly ILogger<DownloadViewModel> _logger;
    private readonly IBiliTokenResolver _tokenResolver;
    private readonly IBiliCookiesResolver _cookiesResolver;
    private readonly string _bbdownPath;
    private readonly string _ffmpegPath;
    private readonly string _downloadPath;
    private readonly bool _openFolderAfterDownload;
    private string _currentUrl;
    private string _parentUrl;
    private int _currentPartIndex;

    [ObservableProperty]
    private List<PlayerFormatInformation> _formats;

    [ObservableProperty]
    private List<VideoPart>? _parts;

    [ObservableProperty]
    private List<EpisodeInformation>? _episodes;

    [ObservableProperty]
    private bool _hasAvailableSubtitle;

    /// <summary>
    /// 元数据初始化完成事件.
    /// </summary>
    public event EventHandler MetaInitialized;
}
