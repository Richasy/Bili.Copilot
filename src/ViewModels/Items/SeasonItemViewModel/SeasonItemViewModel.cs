// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 剧集条目视图模型.
/// </summary>
public sealed partial class SeasonItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonItemViewModel"/> class.
    /// </summary>
    public SeasonItemViewModel(
        SeasonInformation information,
        Action<SeasonItemViewModel> action = default)
    {
        Data = information;
        _additionalAction = action;
        InitializeData();
    }

    [RelayCommand]
    private void Play()
            => AppViewModel.Instance.OpenPlayerCommand.Execute(new PlaySnapshot(default, Data.Identifier.Id, VideoType.Pgc)
            {
                Title = Data.Identifier.Title,
            });

    [RelayCommand]
    private async Task OpenInBrowserAsync()
    {
        var uri = $"https://www.bilibili.com/bangumi/play/ss{Data.Identifier.Id}";
        _ = await Launcher.LaunchUriAsync(new Uri(uri));
    }

    [RelayCommand]
    private async Task UnfollowAsync()
    {
        var result = await FavoriteProvider.RemoveFavoritePgcAsync(Data.Identifier.Id);
        if (result)
        {
            _additionalAction?.Invoke(this);
        }
    }

    [RelayCommand]
    private async Task ChangeFavoriteStatusAsync(int status)
    {
        var result = await FavoriteProvider.UpdateFavoritePgcStatusAsync(Data.Identifier.Id, status);
        if (result)
        {
            _additionalAction?.Invoke(this);
        }
    }

    private void InitializeData()
    {
        IsShowRating = Data.CommunityInformation?.Score > 0;

        if (Data.CommunityInformation?.TrackCount > 0)
        {
            TrackCountText = NumberToolkit.GetCountText(Data.CommunityInformation.TrackCount);
        }
    }
}
