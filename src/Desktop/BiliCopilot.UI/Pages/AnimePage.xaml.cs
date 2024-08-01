// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 动漫页面.
/// </summary>
public sealed partial class AnimePage : AnimePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimePage"/> class.
    /// </summary>
    public AnimePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);

    private void OnNavVisibilityButtonClick(object sender, EventArgs e)
        => ViewModel.IsNavColumnManualHide = !ViewModel.IsNavColumnManualHide;
}

/// <summary>
/// 动漫页面基类.
/// </summary>
public abstract class AnimePageBase : LayoutPageBase<AnimePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimePageBase"/> class.
    /// </summary>
    protected AnimePageBase() => ViewModel = this.Get<AnimePageViewModel>();
}
