// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// PGC 播放器页面.
/// </summary>
public sealed partial class PgcPlayerPage : PgcPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPage"/> class.
    /// </summary>
    public PgcPlayerPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is MediaIdentifier id)
        {
            ViewModel.InitializePageCommand.Execute(id);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => ViewModel.CleanCommand.Execute(default);
}

/// <summary>
/// PGC 播放器页面基类.
/// </summary>
public abstract class PgcPlayerPageBase : LayoutPageBase<PgcPlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPageBase"/> class.
    /// </summary>
    protected PgcPlayerPageBase() => ViewModel = this.Get<PgcPlayerPageViewModel>();
}
