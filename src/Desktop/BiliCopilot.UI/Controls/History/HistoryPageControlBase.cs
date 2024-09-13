// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 历史记录页头部基类.
/// </summary>
public abstract class HistoryPageControlBase : LayoutUserControlBase<HistoryPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryPageControlBase"/> class.
    /// </summary>
    protected HistoryPageControlBase() => ViewModel = this.Get<HistoryPageViewModel>();
}
