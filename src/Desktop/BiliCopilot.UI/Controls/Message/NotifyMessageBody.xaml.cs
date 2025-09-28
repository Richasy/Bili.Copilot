// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 通知消息主体.
/// </summary>
public sealed partial class NotifyMessageBody : NotifyMessageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessageBody"/> class.
    /// </summary>
    public NotifyMessageBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnMessageListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(NotifyMessageSectionDetailViewModel? oldValue, NotifyMessageSectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnMessageListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnMessageListUpdatedAsync;
        View?.ResetScrollPosition();
    }

    private async void OnMessageListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
