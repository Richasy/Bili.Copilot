// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 直播分区详情模块.
/// </summary>
public sealed partial class LivePartitionDetailModule : LivePartitionDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionDetailModule"/> class.
    /// </summary>
    public LivePartitionDetailModule()
    {
        InitializeComponent();
        ViewModel = LivePartitionDetailViewModel.Instance;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.RequestScrollToTop -= OnRequestScrollToTop;

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.RequestScrollToTop += OnRequestScrollToTop;

    private void OnRequestScrollToTop(object sender, EventArgs e)
        => ContentScrollViewer.ChangeView(default, 0, default, true);

    private void OnDetailNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var data = args.InvokedItem as LiveTag;
        _ = ContentScrollViewer.ChangeView(default, 0, default, true);
        ViewModel.SelectTagCommand.Execute(data);
    }

    private void OnLiveViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="LivePartitionDetailModule"/> 的基类.
/// </summary>
public abstract class LivePartitionDetailModuleBase : ReactiveUserControl<LivePartitionDetailViewModel>
{
}
