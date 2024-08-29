// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.ViewLater;

/// <summary>
/// 稍后再看页面控件基类.
/// </summary>
public abstract class ViewLaterPageControlBase : LayoutUserControlBase<ViewLaterPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterPageControlBase"/> class.
    /// </summary>
    protected ViewLaterPageControlBase() => ViewModel = this.Get<ViewLaterPageViewModel>();
}
