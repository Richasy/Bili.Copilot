// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 聊天会话消息主体.
/// </summary>
public sealed partial class ChatSessionBody : ChatMessageControlBase
{
    private long _viewModelChangedToken;
    private ChatMessageSectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionBody"/> class.
    /// </summary>
    public ChatSessionBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.RequestScrollToBottom += OnRequestScrollToBottom;
        OnRequestScrollToBottom(default, default);
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestScrollToBottom -= OnRequestScrollToBottom;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.RequestScrollToBottom -= OnRequestScrollToBottom;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.RequestScrollToBottom += OnRequestScrollToBottom;
    }

    private void OnRequestScrollToBottom(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            MessageView.ScrollTo(0, MessageView.ExtentHeight + MessageView.ViewportHeight + 100, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
        });
    }
}
