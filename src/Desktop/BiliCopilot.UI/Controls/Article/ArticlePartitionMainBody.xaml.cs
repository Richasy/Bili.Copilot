// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Article;

/// <summary>
/// 文章分区主体.
/// </summary>
public sealed partial class ArticlePartitionMainBody : ArticlePartitionDetailControlBase
{
    private long _viewModelChangedToken;
    private ArticlePartitionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionMainBody"/> class.
    /// </summary>
    public ArticlePartitionMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ArticleScrollView.ViewChanged += OnViewChanged;
        ArticleScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ArticleListUpdated += OnArticleListUpdatedAsync;
        CheckArticleCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ArticleListUpdated -= OnArticleListUpdatedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        ArticleScrollView.ViewChanged -= OnViewChanged;
        ArticleScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ArticleListUpdated -= OnArticleListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ArticleListUpdated += OnArticleListUpdatedAsync;
    }

    private async void OnArticleListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckArticleCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (ArticleScrollView.ExtentHeight - ArticleScrollView.ViewportHeight - ArticleScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadArticlesCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckArticleCount();
        }
    }

    private void CheckArticleCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (ArticleScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadArticlesCommand.Execute(default);
            }
        });
    }
}
