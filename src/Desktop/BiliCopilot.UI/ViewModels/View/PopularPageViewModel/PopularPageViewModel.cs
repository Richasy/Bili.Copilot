// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 流行视频页面视图模型.
/// </summary>
public sealed partial class PopularPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPageViewModel"/> class.
    /// </summary>
    public PopularPageViewModel(
        IVideoDiscoveryService discoveryService,
        ILogger<PopularPageViewModel> logger)
    {
        _service = discoveryService;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(PopularPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections.Count > 0)
        {
            return;
        }

        Sections.Add(new PopularSectionItemViewModel(FluentIcons.Common.Symbol.Balloon, ResourceToolkit.GetLocalizedString(StringNames.Recommend), PopularSectionType.Recommend));
        Sections.Add(new PopularSectionItemViewModel(FluentIcons.Common.Symbol.Fire, ResourceToolkit.GetLocalizedString(StringNames.Hot), PopularSectionType.Hot));
        Sections.Add(new PopularSectionItemViewModel(FluentIcons.Common.Symbol.RibbonStar, ResourceToolkit.GetLocalizedString(StringNames.Rank), PopularSectionType.Rank));
        await LoadPartitionsAsync();
        await Task.Delay(200);
        var lastSelectedSectionId = SettingsToolkit.ReadLocalSetting(SettingNames.PopularPageLastSelectedSectionId, PopularSectionType.Recommend.ToString());
        if (int.TryParse(lastSelectedSectionId, out var partitionId))
        {
            var section = Sections.OfType<PopularRankPartitionViewModel>().FirstOrDefault(p => p.Data.Id == partitionId.ToString());
            if (section != null)
            {
                SelectSectionCommand.Execute(section);
            }
            else
            {
                SelectSectionCommand.Execute(Sections.First());
            }
        }
        else if (Enum.TryParse<PopularSectionType>(lastSelectedSectionId, out var sectionType))
        {
            var section = Sections.OfType<PopularSectionItemViewModel>().First(p => p.Type == sectionType);
            SelectSectionCommand.Execute(section);
        }
        else
        {
            SelectSectionCommand.Execute(Sections.First());
        }

        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectSection(IPopularSectionItemViewModel vm)
    {
        if (vm is null || vm == SelectedSection)
        {
            return;
        }

        if (Videos.Count > 0)
        {
            _videoCache[SelectedSection] = Videos.ToList();
        }

        SelectedSection = vm;
        var sectionId = vm switch
        {
            PopularSectionItemViewModel section => section.Type.ToString(),
            PopularRankPartitionViewModel partition => partition.Data.Id,
            _ => string.Empty
        };
        SettingsToolkit.WriteLocalSetting(SettingNames.PopularPageLastSelectedSectionId, sectionId);

        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Videos.Clear();
            if (_videoCache.TryGetValue(vm, out var cacheVideos))
            {
                foreach (var video in cacheVideos)
                {
                    Videos.Add(video);
                }

                VideoListUpdated?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await LoadVideosAsync();
            }
        });
    }

    [RelayCommand]
    private async Task LoadVideosAsync()
    {
        if (IsVideoLoading)
        {
            return;
        }

        IsVideoLoading = true;
        if (SelectedSection is PopularSectionItemViewModel section)
        {
            if (section.Type == PopularSectionType.Recommend)
            {
                await LoadRecommendVideosAsync();
            }
            else if (section.Type == PopularSectionType.Hot)
            {
                await LoadHotVideosAsync();
            }
            else if (section.Type == PopularSectionType.Rank && Videos.Count == 0)
            {
                await LoadTotalRankVideosAsync();
            }
        }
        else if (SelectedSection is PopularRankPartitionViewModel partition && Videos.Count == 0)
        {
            await LoadPartitionRankVideosAsync(partition.Data);
        }

        VideoListUpdated?.Invoke(this, EventArgs.Empty);
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, () => IsVideoLoading = false);
    }

    [RelayCommand]
    private async Task RefreshVideoAsync()
    {
        Videos.Clear();
        _videoCache.Remove(SelectedSection);
        if (SelectedSection is PopularSectionItemViewModel section)
        {
            if (section.Type == PopularSectionType.Recommend)
            {
                _recommendOffset = 0;
            }
            else if (section.Type == PopularSectionType.Hot)
            {
                _hotOffset = 0;
            }
        }

        await LoadVideosAsync();
    }

    [RelayCommand]
    private async Task PlayCuratedListAsync()
    {
        try
        {
            var videos = await _service.GetCuratedPlaylistAsync();
            var firstSnapshot = new VideoSnapshot(videos.First());
            this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage), (videos, firstSnapshot));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取精选视频时失败");
        }
    }
}
