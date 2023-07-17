// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 直播分区索引模块.
/// </summary>
public sealed partial class LivePartitionIndexModule : LivePartitionIndexModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionIndexModule"/> class.
    /// </summary>
    public LivePartitionIndexModule()
    {
        InitializeComponent();
        ViewModel = LivePartitionIndexViewModel.Instance;
    }

    private void OnParentItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        ContentScrollViewer.ChangeView(default, 0, default, true);
        var data = args.InvokedItem as Partition;
        if (data != ViewModel.CurrentParentPartition)
        {
            ViewModel.SelectPartitionCommand.Execute(data);
        }
    }

    private void OnPartitionItemClick(object sender, RoutedEventArgs e)
    {
        var context = (Partition)((FrameworkElement)sender).DataContext;
        ViewModel.OpenPartitionCommand.Execute(context);
    }
}

/// <summary>
/// <see cref="LivePartitionIndexModule"/> 的基类.
/// </summary>
public abstract class LivePartitionIndexModuleBase : ReactiveUserControl<LivePartitionIndexViewModel>
{
}
