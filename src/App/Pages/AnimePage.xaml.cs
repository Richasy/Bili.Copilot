// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Controls;

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
        AnimeTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;
        ViewModel.InitializeCommand.Execute(default);
    }

    private async void OnAnimeTypeSegmentedSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        ContentScrollViewer.ChangeView(default, 0, default, true);
        await Task.Delay(100);
        ViewModel.CurrentType = (AnimeDisplayType)AnimeTypeSelection.SelectedIndex;
    }

    private void OnSeasonViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnTimelineNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        => ViewModel.SelectedTimeline = (TimelineInformation)args.InvokedItem;
}

/// <summary>
/// <see cref="AnimePage"/> 的基类.
/// </summary>
public abstract class AnimePageBase : PageBase<AnimePageViewModel>
{
}
