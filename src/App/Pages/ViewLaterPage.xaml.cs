// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 观看历史页面.
/// </summary>
public sealed partial class ViewLaterPage : ViewLaterPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterPage"/> class.
    /// </summary>
    public ViewLaterPage()
    {
        InitializeComponent();
        ViewModel = ViewLaterDetailViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// <see cref="ViewLaterPage"/> 的基类.
/// </summary>
public abstract class ViewLaterPageBase : PageBase<ViewLaterDetailViewModel>
{
}
