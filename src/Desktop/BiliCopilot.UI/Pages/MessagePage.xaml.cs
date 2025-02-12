// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 消息页面.
/// </summary>
public sealed partial class MessagePage : MessagePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePage"/> class.
    /// </summary>
    public MessagePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 消息页面基类.
/// </summary>
public abstract class MessagePageBase : LayoutPageBase<MessagePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePageBase"/> class.
    /// </summary>
    protected MessagePageBase() => ViewModel = this.Get<MessagePageViewModel>();
}
