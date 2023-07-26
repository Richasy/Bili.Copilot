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
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        // 如果是以暂停状态关闭，可能会导致播放器无法释放.
        if (ViewModel.PlayerDetail.Status == Models.Constants.App.PlayerStatus.Pause)
        {
            ViewModel.PlayerDetail.Player?.Play();
        }

        ViewModel?.Dispose();
    }
}

/// <summary>
/// <see cref="LivePage"/> 的基类.
/// </summary>
public abstract class LivePlayerPageBase : PageBase<LivePlayerPageViewModel>
{
}
