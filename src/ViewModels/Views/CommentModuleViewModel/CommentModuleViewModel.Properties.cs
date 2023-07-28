// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 评论页面/模块视图模型.
/// </summary>
public sealed partial class CommentModuleViewModel
{
    [ObservableProperty]
    private bool _isMainShown;

    [ObservableProperty]
    private bool _isDetailShown;

    /// <summary>
    /// 主视图模型.
    /// </summary>
    public CommentMainModuleViewModel MainViewModel { get; }

    /// <summary>
    /// 详情视图模型.
    /// </summary>
    public CommentDetailModuleViewModel DetailViewModel { get; }
}
