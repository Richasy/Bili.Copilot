// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 分区索引模块.
/// </summary>
public sealed partial class VideoPartitionIndexModule : VideoPartitionIndexModuleBase
{
    private readonly ObservableCollection<int> _placeholderPartitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionIndexModule"/> class.
    /// </summary>
    public VideoPartitionIndexModule()
    {
        InitializeComponent();
        _placeholderPartitions = new ObservableCollection<int>(Enumerable.Range(1, 9).ToList());
        ViewModel = VideoPartitionIndexViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnPartitionItemClick(object sender, RoutedEventArgs e)
    {
        var context = (Partition)((FrameworkElement)sender).DataContext;
        ViewModel.OpenPartitionCommand.Execute(context);
    }
}

/// <summary>
/// <see cref="VideoPartitionIndexModule"/> 的基类.
/// </summary>
public abstract class VideoPartitionIndexModuleBase : ReactiveUserControl<VideoPartitionIndexViewModel>
{
}
