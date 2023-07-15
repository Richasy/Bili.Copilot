// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 分区视图模型.
/// </summary>
public sealed partial class PartitionModuleViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isInDetail;

    /// <summary>
    /// 实例.
    /// </summary>
    public static PartitionModuleViewModel Instance { get; } = new();

    [RelayCommand]
    private void OpenPartitionDetail(Partition partition)
    {
        IsInDetail = true;
        PartitionDetailViewModel.Instance.LoadPartitionCommand.Execute(partition);
    }

    [RelayCommand]
    private void ClosePartitionDetail()
        => IsInDetail = false;

    partial void OnIsInDetailChanged(bool value)
        => AppViewModel.Instance.IsBackButtonShown = value;
}
