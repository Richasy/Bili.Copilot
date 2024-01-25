// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 聊天消息项控件.
/// </summary>
public sealed partial class ChatMessageItemControl : ChatMessageItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemControl"/> class.
    /// </summary>
    public ChatMessageItemControl()
    {
        InitializeComponent();
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ChatMessageItemViewModel vm)
        {
            _ = vm.IsMe
                ? VisualStateManager.GoToState(this, nameof(MyState), false)
                : VisualStateManager.GoToState(this, nameof(AssistantState), false);
        }
    }
}

/// <summary>
/// 聊天消息项控件基类.
/// </summary>
public abstract class ChatMessageItemControlBase : ReactiveUserControl<ChatMessageItemViewModel>
{
}
