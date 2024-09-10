﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 聊天模型卡片控件.
/// </summary>
public sealed class ChatModelCardControl : LayoutControlBase<ChatModelItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelCardControl"/> class.
    /// </summary>
    public ChatModelCardControl()
    {
        DefaultStyleKey = typeof(ChatModelCardControl);
    }
}
