// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 直播间页面.
/// </summary>
public sealed partial class LivePage : LivePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePage"/> class.
    /// </summary>
    public LivePage()
    {
        InitializeComponent();
        ViewModel = LivePageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// <see cref="LivePage"/> 的基类.
/// </summary>
public abstract class LivePageBase : PageBase<LivePageViewModel>
{
}
