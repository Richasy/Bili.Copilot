// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 分区视图模型.
/// </summary>
public sealed partial class VideoPartitionModuleViewModel : ViewModelBase
{
    private static readonly Lazy<VideoPartitionModuleViewModel> _lazyInstance = new(() => new VideoPartitionModuleViewModel());
    private bool _isInitialized;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private double _navListColumnWidth;

    [ObservableProperty]
    private PartitionItemViewModel _currentPartition;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionModuleViewModel"/> class.
    /// </summary>
    private VideoPartitionModuleViewModel()
    {
        Partitions = new ObservableCollection<PartitionItemViewModel>();
        NavListColumnWidth = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.VideoPartitionNavListColumnWidth, 240d);
        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, InitializeCommand);
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static VideoPartitionModuleViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 分区集合.
    /// </summary>
    public ObservableCollection<PartitionItemViewModel> Partitions { get; }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized || IsInitializing)
        {
            return;
        }

        var data = await HomeProvider.GetVideoPartitionIndexAsync();
        data.ToList().ForEach(p => Partitions.Add(new PartitionItemViewModel(p)));

        _isInitialized = true;
    }

    [RelayCommand]
    private void OpenPartitionDetail(PartitionItemViewModel partition)
    {
        CurrentPartition = partition;
        foreach (var item in Partitions)
        {
            item.IsSelected = item.Data.Id == CurrentPartition.Data.Id;
        }

        VideoPartitionDetailViewModel.Instance.LoadPartitionCommand.Execute(partition.Data);
    }

    partial void OnNavListColumnWidthChanged(double value)
    {
        if (value >= 240)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.VideoPartitionNavListColumnWidth, value);
        }
    }
}
