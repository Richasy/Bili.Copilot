// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 聊天会话消息主体.
/// </summary>
public sealed partial class ChatSessionBody : ChatMessageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionBody"/> class.
    /// </summary>
    public ChatSessionBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel is null)
        {
            return;
        }

        OnRequestScrollToBottom(default, default);
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestScrollToBottom -= OnRequestScrollToBottom;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatMessageSectionDetailViewModel? oldValue, ChatMessageSectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.RequestScrollToBottom -= OnRequestScrollToBottom;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.RequestScrollToBottom += OnRequestScrollToBottom;
    }

    private void OnRequestScrollToBottom(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            MessageView.ScrollTo(0, MessageView.ExtentHeight + MessageView.ViewportHeight + 100, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
        });
    }
}
