// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 影视圈页面.
/// </summary>
public sealed partial class FilmPage : FilmPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilmPage"/> class.
    /// </summary>
    public FilmPage()
    {
        InitializeComponent();
        ViewModel = FilmPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ViewModel.InitializeCommand.Execute(default);
    }
}

/// <summary>
/// <see cref="FilmPage"/> 的基类.
/// </summary>
public abstract class FilmPageBase : PageBase<FilmPageViewModel>
{
}
