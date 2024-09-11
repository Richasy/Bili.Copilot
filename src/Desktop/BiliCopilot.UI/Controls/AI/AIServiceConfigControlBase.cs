// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// 聊天服务配置控件基类.
/// </summary>
public abstract class AIServiceConfigControlBase : LayoutUserControlBase<AIServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIServiceConfigControlBase"/> class.
    /// </summary>
    protected AIServiceConfigControlBase() => IsTabStop = false;
}
