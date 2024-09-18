// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

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
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        CommentScrollView.ViewChanged += OnViewChanged;
        CommentScrollView.SizeChanged += OnScrollViewSizeChanged;
        if (ViewModel is null)
        {
            return;
        }

        CheckCommentCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnCommentListUpdatedAsync;
        }

        CommentScrollView.ViewChanged -= OnViewChanged;
        CommentScrollView.SizeChanged -= OnScrollViewSizeChanged;
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
        await Task.Delay(500);
        CheckCommentCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (CommentScrollView.ExtentHeight - CommentScrollView.ViewportHeight - CommentScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckCommentCount();
        }
    }

    private void CheckCommentCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (CommentScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 评论详情面板基类.
/// </summary>
public abstract class CommentDetailPanelBase : LayoutUserControlBase<CommentDetailViewModel>
{
}
