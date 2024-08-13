// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 直播播放页面.
/// </summary>
public sealed partial class LivePlayerPage : LivePlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPage"/> class.
    /// </summary>
    public LivePlayerPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is LiveInformation live)
        {
            ViewModel.InitializePageCommand.Execute(live.Identifier);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => ViewModel.CleanCommand.Execute(default);
}

/// <summary>
/// 直播播放页面基类.
/// </summary>
public abstract class LivePlayerPageBase : LayoutPageBase<LivePlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPageBase"/> class.
    /// </summary>
    protected LivePlayerPageBase() => ViewModel = this.Get<LivePlayerPageViewModel>();
}
