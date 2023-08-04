// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 下载模块视图模型.
/// </summary>
public sealed partial class DownloadModuleViewModel
{
    private string _id;

    [ObservableProperty]
    private bool _isSupported;

    [ObservableProperty]
    private bool _isMultiPartShown;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private bool _openFolderWhenDownloaded;

    /// <summary>
    /// 分集列表.
    /// </summary>
    public ObservableCollection<VideoIdentifierSelectableViewModel> Parts { get; }
}
