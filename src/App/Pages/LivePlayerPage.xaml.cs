// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Navigation;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 直播播放器页面.
/// </summary>
public sealed partial class LivePlayerPage : LivePlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPage"/> class.
    /// </summary>
    public LivePlayerPage()
    {
        InitializeComponent();
        ViewModel = new LivePlayerPageViewModel();
        DataContext = ViewModel;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is PlayerPageNavigateEventArgs args)
        {
            ViewModel.SetWindow(args.AttachedWindow);
            ViewModel.SetSnapshot(args.Snapshot);
        }
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
    {
        ViewModel?.Dispose();
    }

    private void OnSectionHeaderItemInvoked(object sender, Models.App.Other.PlayerSectionHeader e)
    {
        if (ViewModel.CurrentSection != e)
        {
            ViewModel.CurrentSection = e;
        }
    }
}

/// <summary>
/// <see cref="LivePage"/> 的基类.
/// </summary>
public abstract class LivePlayerPageBase : PageBase<LivePlayerPageViewModel>
{
}
