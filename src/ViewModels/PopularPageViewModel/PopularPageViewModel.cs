// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 流行视频页面的视图模型.
/// </summary>
public sealed partial class PopularPageViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPageViewModel"/> class.
    /// </summary>
    private PopularPageViewModel()
    {
        _caches = new Dictionary<PopularType, IEnumerable<VideoInformation>>();
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastPopularType, PopularType.Recommend);
        CheckModuleState();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        if (IsRecommendShown)
        {
            HomeProvider.Instance.ResetRecommendState();
        }
        else if (IsHotShown)
        {
            HomeProvider.Instance.ResetHotState();
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
    {
        var prefix = CurrentType switch
        {
            PopularType.Recommend => ResourceToolkit.GetLocalizedString(StringNames.RequestRecommendFailed),
            PopularType.Hot => ResourceToolkit.GetLocalizedString(StringNames.RequestPopularFailed),
            PopularType.Rank => ResourceToolkit.GetLocalizedString(StringNames.RankRequestFailed),
            _ => string.Empty,
        };
        return $"{prefix}\n{errorMsg}";
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        IEnumerable<VideoInformation> videos = default;
        switch (CurrentType)
        {
            case PopularType.Recommend:
                var items = await HomeProvider.Instance.RequestRecommendVideosAsync();
                videos = items.OfType<VideoInformation>();
                break;
            case PopularType.Hot:
                videos = await HomeProvider.Instance.RequestHotVideosAsync();
                break;
            case PopularType.Rank:
                videos = await HomeProvider.GetRankDetailAsync("0");
                break;
        }

        if (videos?.Count() > 0)
        {
            IsEmpty = false;
            foreach (var video in videos)
            {
                video.Publisher = default;
                var videoVM = new VideoItemViewModel(video);
                Items.Add(videoVM);
            }

            var videoVMs = Items
                    .OfType<VideoItemViewModel>()
                    .Select(p => p.Data)
                    .ToList();
            if (_caches.ContainsKey(CurrentType))
            {
                _caches[CurrentType] = videoVMs;
            }
            else
            {
                _caches.Add(CurrentType, videoVMs);
            }
        }
        else
        {
            IsEmpty = Items.Count == 0;
        }
    }

    private void CheckModuleState()
    {
        IsRecommendShown = CurrentType == PopularType.Recommend;
        IsHotShown = CurrentType == PopularType.Hot;
        IsRankShown = CurrentType == PopularType.Rank;

        Title = CurrentType switch
        {
            PopularType.Recommend => ResourceToolkit.GetLocalizedString(StringNames.Recommend),
            PopularType.Hot => ResourceToolkit.GetLocalizedString(StringNames.Hot),
            PopularType.Rank => ResourceToolkit.GetLocalizedString(StringNames.Rank),
            _ => string.Empty,
        };
    }

    partial void OnCurrentTypeChanged(PopularType value)
    {
        CheckModuleState();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastPopularType, value);

        if (!IsInitialized)
        {
            return;
        }

        TryClear(Items);
        if (_caches.ContainsKey(value))
        {
            var data = _caches[value];
            foreach (var video in data)
            {
                var videoVM = new VideoItemViewModel(video);
                Items.Add(videoVM);
            }

            IsEmpty = Items.Count == 0;
        }
        else
        {
            InitializeCommand.ExecuteAsync(default);
        }
    }
}
