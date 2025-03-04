// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 消息页面控件基类.
/// </summary>
public abstract class MessagePageControlBase : LayoutUserControlBase<MessagePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePageControlBase"/> class.
    /// </summary>
    protected MessagePageControlBase() => ViewModel = this.Get<MessagePageViewModel>();
}
