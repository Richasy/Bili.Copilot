// Copyright (c) Bili Copilot. All rights reserved.
namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频动态评论视图模型.
/// </summary>
public sealed partial class CommentViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentViewModel"/> class.
    /// </summary>
    public CommentViewModel()
    {
        Comments = new CommentModuleViewModel();
    }
}
