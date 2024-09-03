// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.WebDav;

/// <summary>
/// WebDAV 存储项控件.
/// </summary>
public sealed partial class WebDavStorageItemControl : WebDavStorageItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavStorageItemControl"/> class.
    /// </summary>
    public WebDavStorageItemControl() => InitializeComponent();
}

/// <summary>
/// WebDAV 存储项控件基类.
/// </summary>
public abstract class WebDavStorageItemControlBase : LayoutUserControlBase<WebDavStorageItemViewModel>
{
}
