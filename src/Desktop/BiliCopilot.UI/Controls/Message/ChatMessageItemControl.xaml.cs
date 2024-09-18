// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 聊天消息项控件.
/// </summary>
public sealed partial class ChatMessageItemControl : ChatMessageItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemControl"/> class.
    /// </summary>
    public ChatMessageItemControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded() => CheckState();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatMessageItemViewModel? oldValue, ChatMessageItemViewModel? newValue)
        => CheckState();

    private void CheckState()
    {
        if (ViewModel is null)
        {
            return;
        }

        _ = ViewModel.IsMe
                ? VisualStateManager.GoToState(this, nameof(MyState), false)
                : VisualStateManager.GoToState(this, nameof(AssistantState), false);
    }
}

/// <summary>
/// 聊天消息项控件基类.
/// </summary>
public abstract class ChatMessageItemControlBase : LayoutUserControlBase<ChatMessageItemViewModel>
{
}
