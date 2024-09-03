﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.WebDav;

/// <summary>
/// WebDAV 页面控件基类.
/// </summary>
public abstract class WebDavPageControlBase : LayoutUserControlBase<WebDavPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPageControlBase"/> class.
    /// </summary>
    protected WebDavPageControlBase() => ViewModel = this.Get<WebDavPageViewModel>();
}
