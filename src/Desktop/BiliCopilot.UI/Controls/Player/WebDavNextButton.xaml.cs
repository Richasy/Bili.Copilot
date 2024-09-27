// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// WebDAV 下一个按钮.
/// </summary>
public sealed partial class WebDavNextButton : WebDavNextButtonBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavNextButton"/> class.
    /// </summary>
    public WebDavNextButton() => InitializeComponent();
}

/// <summary>
/// WebDAV 下一个按钮基类.
/// </summary>
public abstract class WebDavNextButtonBase : LayoutUserControlBase<WebDavPlayerPageViewModel>
{
}
