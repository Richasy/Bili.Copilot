// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Navigation;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// PGC 内容播放页面.
/// </summary>
public sealed partial class PgcPlayerPage : PgcPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPage"/> class.
    /// </summary>
    public PgcPlayerPage()
    {
        InitializeComponent();
        ViewModel = new PgcPlayerPageViewModel();
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
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        // 如果是以暂停状态关闭，可能会导致播放器无法释放.
        if (ViewModel.PlayerDetail.Status == Models.Constants.App.PlayerStatus.Pause)
        {
            ViewModel.PlayerDetail.Player?.Play();
        }
        else if (ViewModel.PlayerDetail.Status is Models.Constants.App.PlayerStatus.End or Models.Constants.App.PlayerStatus.NotLoad)
        {
            ViewModel.PlayerDetail.ChangeProgressCommand.Execute(0);
            await Task.Delay(1000);
            ViewModel.PlayerDetail.Player?.Play();
        }

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
/// <see cref="PgcPlayerPage"/> 的基类.
/// </summary>
public abstract class PgcPlayerPageBase : PageBase<PgcPlayerPageViewModel>
{
}
