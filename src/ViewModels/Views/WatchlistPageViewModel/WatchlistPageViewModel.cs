﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 观看列表页面的视图模型.
/// </summary>
public sealed partial class WatchlistPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WatchlistPageViewModel"/> class.
    /// </summary>
    private WatchlistPageViewModel()
    {
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastWatchlistType, WatchlistType.ViewLater);
        CheckModuleStateAsync();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand, InitializeCommand);
        AttachIsRunningToAsyncCommand(p => IsClearing = p, ClearAllCommand);
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsHistoryShown)
        {
            await HistoryDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
        else if (IsViewLaterShown)
        {
            await ViewLaterDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
        else
        {
            await VideoFavoriteDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await InitializeCurrentModuleAsync();
        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        if (IsHistoryShown)
        {
            await HistoryDetailViewModel.Instance.ClearAllCommand.ExecuteAsync(default);
        }
        else if (IsViewLaterShown)
        {
            await ViewLaterDetailViewModel.Instance.ClearAllCommand.ExecuteAsync(default);
        }
    }

    [RelayCommand]
    private void PlayAll()
    {
        if (IsViewLaterShown)
        {
            ViewLaterDetailViewModel.Instance.PlayAllCommand.Execute(default);
        }
    }

    private async Task InitializeCurrentModuleAsync()
    {
        if (IsHistoryShown)
        {
            await HistoryDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
        else if (IsViewLaterShown)
        {
            await ViewLaterDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
        else
        {
            await VideoFavoriteDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
    }

    private async void CheckModuleStateAsync()
    {
        IsViewLaterShown = CurrentType == WatchlistType.ViewLater;
        IsHistoryShown = CurrentType == WatchlistType.History;
        IsFavoriteShown = CurrentType == WatchlistType.Favorite;

        Title = CurrentType switch
        {
            WatchlistType.ViewLater => ResourceToolkit.GetLocalizedString(StringNames.ViewLater),
            WatchlistType.History => ResourceToolkit.GetLocalizedString(StringNames.ViewHistory),
            WatchlistType.Favorite => ResourceToolkit.GetLocalizedString(StringNames.Favorite),
            _ => string.Empty,
        };

        await InitializeCurrentModuleAsync();
    }

    partial void OnCurrentTypeChanged(WatchlistType value)
    {
        CheckModuleStateAsync();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastWatchlistType, value);
    }
}
