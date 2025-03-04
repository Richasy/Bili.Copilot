// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 稍后再看页面.
/// </summary>
public sealed partial class ViewLaterPage : ViewLaterPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterPage"/> class.
    /// </summary>
    public ViewLaterPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 稍后再看页面基类.
/// </summary>
public abstract class ViewLaterPageBase : LayoutPageBase<ViewLaterPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterPageBase"/> class.
    /// </summary>
    protected ViewLaterPageBase() => ViewModel = this.Get<ViewLaterPageViewModel>();
}
