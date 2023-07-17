// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播推荐详情的视图模型.
/// </summary>
public sealed partial class LiveRecommendDetailViewModel : InformationFlowViewModel<LiveItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveRecommendDetailViewModel"/> class.
    /// </summary>
    private LiveRecommendDetailViewModel()
    {
        Follows = new ObservableCollection<LiveItemViewModel>();
        IsFollowsVisible = SettingsToolkit.ReadLocalSetting(SettingNames.IsLiveFollowsVisible, true);
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        Follows.Clear();
        LiveProvider.Instance.ResetFeedState();
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestLiveFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        var data = await LiveProvider.Instance.GetLiveFeedsAsync();

        if (data.RecommendLives.Any())
        {
            foreach (var item in data.RecommendLives)
            {
                var liveVM = new LiveItemViewModel(item);
                Items.Add(liveVM);
            }
        }

        if (data.FollowLives.Any())
        {
            foreach (var item in data.FollowLives)
            {
                if (Follows.Any(p => p.Data.Equals(item)))
                {
                    continue;
                }

                var liveVM = new LiveItemViewModel(item);
                Follows.Add(liveVM);
            }
        }

        IsFollowsEmpty = Follows.Count == 0;
    }

    [RelayCommand]
    private void ToggleFollowsVisibility()
        => IsFollowsVisible = !IsFollowsVisible;

    partial void OnIsFollowsVisibleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsLiveFollowsVisible, value);
}
