// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 评论项视图模型.
/// </summary>
public sealed partial class CommentLikeButton : CommentLikeButtonBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentLikeButton"/> class.
    /// </summary>
    public CommentLikeButton() => InitializeComponent();
}

/// <summary>
/// 评论点赞按钮基类.
/// </summary>
public abstract class CommentLikeButtonBase : LayoutUserControlBase<CommentItemViewModel>
{
}
