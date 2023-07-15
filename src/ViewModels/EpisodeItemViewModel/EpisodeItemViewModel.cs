// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 剧集单集视图模型.
/// </summary>
public sealed partial class EpisodeItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeItemViewModel"/> class.
    /// </summary>
    /// <param name="information">剧集信息.</param>
    public EpisodeItemViewModel(EpisodeInformation information)
    {
        Data = information;
        InitializeData();
    }

    [RelayCommand]
    private void Play()
    {
        var snapshot = new PlaySnapshot(Data.Identifier.Id, Data.SeasonId, VideoType.Pgc)
        {
            Title = Data.Identifier.Title,
        };

        AppViewModel.Instance.OpenPlayerCommand.Execute(snapshot);
    }

    [RelayCommand]
    private async Task OpenInBrowserAsync()
    {
        var uri = $"https://www.bilibili.com/bangumi/play/ep{Data.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void InitializeData()
    {
        if (Data.CommunityInformation != null)
        {
            PlayCountText = NumberToolkit.GetCountText(Data.CommunityInformation.PlayCount);
            DanmakuCountText = NumberToolkit.GetCountText(Data.CommunityInformation.DanmakuCount);
        }

        if (Data.Identifier.Duration > 0)
        {
            DurationText = NumberToolkit.GetDurationText(TimeSpan.FromSeconds(Data.Identifier.Duration));
        }
    }
}
