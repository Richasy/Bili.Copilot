// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 我的关注详情.
/// </summary>
public sealed partial class MyFollowDetailModule : MyFollowDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyFollowDetailModule"/> class.
    /// </summary>
    public MyFollowDetailModule() => InitializeComponent();

    private void OnIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnNavItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var data = args.InvokedItem as FollowGroup;
        if (data != ViewModel.CurrentGroup)
        {
            ViewModel.SelectGroupCommand.Execute(data);
        }
    }
}

/// <summary>
/// <see cref="MyFollowDetailModule"/> 的基类.
/// </summary>
public abstract class MyFollowDetailModuleBase : ReactiveUserControl<MyFollowsDetailViewModel>
{
}
