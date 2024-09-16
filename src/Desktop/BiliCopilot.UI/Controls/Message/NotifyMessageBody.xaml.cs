// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 通知消息主体.
/// </summary>
public sealed partial class NotifyMessageBody : NotifyMessageControlBase
{
    private long _viewModelChangedToken;
    private NotifyMessageSectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessageBody"/> class.
    /// </summary>
    public NotifyMessageBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        MessageScrollView.ViewChanged += OnViewChanged;
        MessageScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ListUpdated += OnMessageListUpdatedAsync;
        CheckMessageCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnMessageListUpdatedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        MessageScrollView.ViewChanged -= OnViewChanged;
        MessageScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ListUpdated -= OnMessageListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ListUpdated += OnMessageListUpdatedAsync;
        MessageScrollView.ChangeView(0, 0, default);
    }

    private async void OnMessageListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckMessageCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (MessageScrollView.ExtentHeight - MessageScrollView.ViewportHeight - MessageScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckMessageCount();
        }
    }

    private void CheckMessageCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (MessageScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}
