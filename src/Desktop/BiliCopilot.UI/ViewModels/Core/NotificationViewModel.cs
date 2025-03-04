// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel : INotificationViewModel
{
    [RelayCommand]
    private static async Task ShowTipAsync((string, InfoType) data)
        => await GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().ShowTipCommand.ExecuteAsync(data);
}
