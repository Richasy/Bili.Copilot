// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Message;

/// <summary>
/// 通知消息卡片控件.
/// </summary>
public sealed partial class NotifyMessageCardControl : NotifyMessageCardControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessageCardControl"/> class.
    /// </summary>
    public NotifyMessageCardControl()
    {
        InitializeComponent();
    }
}

/// <summary>
/// 通知消息卡片控件.
/// </summary>
public abstract class NotifyMessageCardControlBase : LayoutUserControlBase<NotifyMessageItemViewModel>;
