// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 收藏页面.
/// </summary>
public sealed partial class FavoritesPage : FavoritesPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPage"/> class.
    /// </summary>
    public FavoritesPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnPageLoaded() => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 收藏页面基类.
/// </summary>
public abstract class FavoritesPageBase : LayoutPageBase<FavoritesPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPageBase"/> class.
    /// </summary>
    protected FavoritesPageBase() => ViewModel = this.Get<FavoritesPageViewModel>();
}
