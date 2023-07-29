// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 主页.
/// </summary>
public sealed partial class HomePage : PageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HomePage"/> class.
    /// </summary>
    public HomePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded() => CoreViewModel.IsBackButtonShown = false;
}
