// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Comment;

/// <summary>
/// 评论详情面板.
/// </summary>
public sealed partial class CommentMainPanel : CommentMainPanelBase
{
    private long _viewModelChangedToken;
    private CommentMainViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentMainPanel"/> class.
    /// </summary>
    public CommentMainPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        CommentScrollView.ViewChanged += OnViewChanged;
        CommentScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.Initialized += OnInitialized;
        ViewModel.ListUpdated += OnCommentListUpdatedAsync;

        CheckSortType();
        CheckCommentCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnCommentListUpdatedAsync;
            ViewModel.Initialized -= OnInitialized;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        CommentScrollView.ViewChanged -= OnViewChanged;
        CommentScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ListUpdated -= OnCommentListUpdatedAsync;
            ViewModel.Initialized -= OnInitialized;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.Initialized += OnInitialized;
        _viewModel.ListUpdated += OnCommentListUpdatedAsync;
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

    private void OnInitialized(object? sender, EventArgs e)
        => CheckSortType();

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
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
