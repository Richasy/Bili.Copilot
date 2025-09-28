// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Comment;

/// <summary>
/// 评论详情面板.
/// </summary>
public sealed partial class CommentDetailPanel : CommentDetailPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDetailPanel"/> class.
    /// </summary>
    public CommentDetailPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnCommentListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(CommentDetailViewModel? oldValue, CommentDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnCommentListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnCommentListUpdatedAsync;
    }

    private async void OnCommentListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}

/// <summary>
/// 评论详情面板基类.
/// </summary>
public abstract class CommentDetailPanelBase : LayoutUserControlBase<CommentDetailViewModel>
{
}
