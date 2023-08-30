// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频动态评论视图模型.
/// </summary>
public sealed partial class CommentViewModel
{
    [ObservableProperty]
    private CommentModuleViewModel _comments;
}
