// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 流行视频页.
/// </summary>
public sealed partial class PopularPage : PopularPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPage"/> class.
    /// </summary>
    public PopularPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
    }

    private void OnNavVisibilityButtonClick(object sender, EventArgs e)
        => ViewModel.IsNavColumnManualHide = !ViewModel.IsNavColumnManualHide;
}

/// <summary>
/// 流行视频页面基类.
/// </summary>
public abstract class PopularPageBase : LayoutPageBase<PopularPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPageBase"/> class.
    /// </summary>
    protected PopularPageBase() => ViewModel = this.Get<PopularPageViewModel>();
}
