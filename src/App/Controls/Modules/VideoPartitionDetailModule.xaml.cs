// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 分区详情模块.
/// </summary>
public sealed partial class VideoPartitionDetailModule : VideoPartitionDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionDetailModule"/> class.
    /// </summary>
    public VideoPartitionDetailModule()
    {
        InitializeComponent();
        ViewModel = VideoPartitionDetailViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => DetailTypeSelection.SelectedIndex = (int)ViewModel.DetailType;

    private void OnShowTypeSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.DetailType = (VideoPartitionDetailType)DetailTypeSelection.SelectedIndex;

    private void OnDetailNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var data = args.InvokedItem as Partition;
        ContentScrollViewer.ChangeView(default, 0, default);
        ViewModel.SelectSubPartitionCommand.Execute(data);
    }

    private void OnVideoSortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (VideoSortComboBox.SelectedItem is VideoSortType type
            && ViewModel.SortType != type
            && ViewModel.IsInitialized)
        {
            ViewModel.SortType = type;
            ContentScrollViewer.ChangeView(default, 0, default);
            ViewModel.ReloadCommand.ExecuteAsync(default);
        }
    }

    private void OnVideoViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// 分区详情模块的基类.
/// </summary>
public abstract class VideoPartitionDetailModuleBase : ReactiveUserControl<VideoPartitionDetailViewModel>
{
}
