// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 动漫页面.
/// </summary>
public sealed partial class AnimePage : AnimePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimePage"/> class.
    /// </summary>
    public AnimePage()
    {
        InitializeComponent();
        ViewModel = AnimePageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ViewModel.InitializeCommand.Execute(default);
    }
}

/// <summary>
/// <see cref="AnimePage"/> 的基类.
/// </summary>
public abstract class AnimePageBase : PageBase<AnimePageViewModel>
{
}
