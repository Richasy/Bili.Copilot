// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 观看列表页面，包括稍后再看、历史记录和视频收藏.
/// </summary>
public sealed partial class WatchlistPage : WatchlistPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WatchlistPage"/> class.
    /// </summary>
    public WatchlistPage()
    {
        InitializeComponent();
        ViewModel = WatchlistPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        WatchlistTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;
        ViewModel.InitializeCommand.Execute(default);
    }

    private void OnWatchlistTypeSegmentedSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        => ViewModel.CurrentType = (WatchlistType)WatchlistTypeSelection.SelectedIndex;
}

/// <summary>
/// <see cref="WatchlistPage"/>的基类.
/// </summary>
public abstract class WatchlistPageBase : PageBase<WatchlistPageViewModel>
{
}
