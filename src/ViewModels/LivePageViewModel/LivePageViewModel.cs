// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播页面视图模型.
/// </summary>
public sealed partial class LivePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePageViewModel"/> class.
    /// </summary>
    public LivePageViewModel()
    {
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastLiveDisplayType, LiveDisplayType.Recommend);
        CheckModuleStateAsync();
        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand, InitializeCommand);
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsRecommendShown)
        {
            await LiveRecommendDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
        else if (IsPartitionShown)
        {
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

    private async Task InitializeCurrentModuleAsync()
    {
        if (IsRecommendShown)
        {
            await LiveRecommendDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
        else if (IsPartitionShown)
        {
        }
    }

    private async void CheckModuleStateAsync()
    {
        IsRecommendShown = CurrentType == LiveDisplayType.Recommend;
        IsPartitionShown = CurrentType == LiveDisplayType.Partition;

        Title = CurrentType switch
        {
            LiveDisplayType.Recommend => ResourceToolkit.GetLocalizedString(StringNames.Live),
            LiveDisplayType.Partition => ResourceToolkit.GetLocalizedString(StringNames.Partition),
            _ => string.Empty,
        };

        await InitializeCurrentModuleAsync();
    }

    partial void OnCurrentTypeChanged(LiveDisplayType value)
    {
        CheckModuleStateAsync();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastLiveDisplayType, value);
    }
}
