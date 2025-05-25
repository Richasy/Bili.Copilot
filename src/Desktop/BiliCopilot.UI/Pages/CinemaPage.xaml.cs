// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 影院页面.
/// </summary>
public sealed partial class CinemaPage : CinemaPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CinemaPage"/> class.
    /// </summary>
    public CinemaPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 影院页面基类.
/// </summary>
public abstract class CinemaPageBase : LayoutPageBase<CinemaPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CinemaPageBase"/> class.
    /// </summary>
    protected CinemaPageBase() => ViewModel = this.Get<CinemaPageViewModel>();
}
