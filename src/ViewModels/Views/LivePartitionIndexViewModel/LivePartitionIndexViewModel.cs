﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播分区索引视图模型.
/// </summary>
public sealed partial class LivePartitionIndexViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionIndexViewModel"/> class.
    /// </summary>
    public LivePartitionIndexViewModel()
    {
        ParentPartitions = new ObservableCollection<PartitionItemViewModel>();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand, InitializeCommand);
    }

    [RelayCommand]
    private static void OpenPartition(Partition partition)
        => LivePartitionDetailViewModel.Instance.SetPartitionCommand.Execute(partition);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (ParentPartitions.Count > 0 || IsReloading)
        {
            return;
        }

        IsReloading = true;
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            LogException(ex);
        }

        IsReloading = false;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        TryClear(ParentPartitions);
        var partitions = await LiveProvider.GetLiveAreaIndexAsync();
        partitions.ToList().ForEach(p => ParentPartitions.Add(new PartitionItemViewModel(p)));
    }

    private void DisplayException(Exception exception)
    {
        IsError = true;
        IsReloading = false;
        var msg = exception is ServiceException se
        ? se.GetMessage()
            : exception.Message;
        ErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestLiveTagsFailed)}\n{msg}";
        LogException(exception);
    }
}
