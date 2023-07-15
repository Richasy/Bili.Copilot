// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 分区索引视图模型.
/// </summary>
public sealed partial class PartitionIndexViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionIndexViewModel"/> class.
    /// </summary>
    private PartitionIndexViewModel()
    {
        Partitions = new ObservableCollection<Partition>();
        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, InitializeCommand);
    }

    [RelayCommand]
    private static void OpenPartition(Partition partition)
        => PartitionModuleViewModel.Instance.OpenPartitionDetailCommand.Execute(partition);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized || IsInitializing)
        {
            return;
        }

        var data = await HomeProvider.GetVideoPartitionIndexAsync();
        data.ToList().ForEach(Partitions.Add);
        _isInitialized = true;
    }
}
