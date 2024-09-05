// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// WebDAV 页面.
/// </summary>
public sealed partial class WebDavPage : WebDavPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPage"/> class.
    /// </summary>
    public WebDavPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// WebDAV 页面基类.
/// </summary>
public abstract class WebDavPageBase : LayoutPageBase<WebDavPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPageBase"/> class.
    /// </summary>
    protected WebDavPageBase() => ViewModel = this.Get<WebDavPageViewModel>();
}
