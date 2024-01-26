// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 消息页面.
/// </summary>
public sealed partial class MessagePage : MessagePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePage"/> class.
    /// </summary>
    public MessagePage()
    {
        InitializeComponent();
        ViewModel = MessageDetailViewModel.Instance;
        ViewModel.InitializeCommand.Execute(default);
    }
}

/// <summary>
/// 消息页面基类.
/// </summary>
public abstract class MessagePageBase : PageBase<MessageDetailViewModel>
{
}
