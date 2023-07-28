// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Comment;

/// <summary>
/// 评论区详情视图.
/// </summary>
public sealed class CommentDetailView : ReactiveControl<CommentDetailModuleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDetailView"/> class.
    /// </summary>
    public CommentDetailView() => DefaultStyleKey = typeof(CommentDetailView);
}
