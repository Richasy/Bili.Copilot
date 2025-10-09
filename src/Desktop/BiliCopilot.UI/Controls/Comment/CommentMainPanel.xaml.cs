// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Comment;

/// <summary>
/// 评论详情面板.
/// </summary>
public sealed partial class CommentMainPanel : CommentMainPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentMainPanel"/> class.
    /// </summary>
    public CommentMainPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel is null)
        {
            return;
        }

        CheckSortType();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnCommentListUpdatedAsync;
            ViewModel.Initialized -= OnInitialized;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(CommentMainViewModel? oldValue, CommentMainViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnCommentListUpdatedAsync;
            oldValue.Initialized -= OnInitialized;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.Initialized += OnInitialized;
        newValue.ListUpdated += OnCommentListUpdatedAsync;
    }

    private async void OnCommentListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }

    private void OnInitialized(object? sender, EventArgs e)
        => CheckSortType();

    private void CheckSortType()
    {
        var selectedIndex = (int)ViewModel.SortType;
        if (SortTypeComboBox.SelectedIndex != selectedIndex)
        {
            SortTypeComboBox.SelectedIndex = selectedIndex;
        }
    }

    private void OnSortChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || SortTypeComboBox.SelectedIndex < 0)
        {
            return;
        }

        var sortType = (CommentSortType)SortTypeComboBox.SelectedIndex;
        if (sortType != ViewModel.SortType)
        {
            ViewModel.ChangeSortTypeCommand.Execute(sortType);
        }
    }
}

/// <summary>
/// 评论主面板基类.
/// </summary>
public abstract class CommentMainPanelBase : LayoutUserControlBase<CommentMainViewModel>
{
}
