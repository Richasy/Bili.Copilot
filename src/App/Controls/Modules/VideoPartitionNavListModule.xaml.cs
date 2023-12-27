// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 视频分区导航列表模块.
/// </summary>
public sealed partial class VideoPartitionNavListModule : VideoPartitionNavListModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionNavListModule"/> class.
    /// </summary>
    public VideoPartitionNavListModule()
    {
        InitializeComponent();
        ViewModel = VideoPartitionModuleViewModel.Instance;
    }

    private void OnPartitionClick(object sender, RoutedEventArgs e)
    {
        var vm = ((PartitionItem)sender).ViewModel;
        ViewModel.OpenPartitionDetailCommand.Execute(vm);
    }
}

/// <summary>
/// 视频分区导航列表模块基类.
/// </summary>
public abstract class VideoPartitionNavListModuleBase : ReactiveUserControl<VideoPartitionModuleViewModel>
{
}
