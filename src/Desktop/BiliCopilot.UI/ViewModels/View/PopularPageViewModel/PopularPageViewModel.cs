﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 流行视频页面视图模型.
/// </summary>
public sealed partial class PopularPageViewModel : ViewModelBase
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

        NavColumnWidth = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PopularPageNavColumnWidth, 240d);
        IsNavColumnManualHide = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsPopularPageNavColumnManualHide, false);
    }

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
        SelectSectionCommand.Execute(Sections.First());
        await LoadPartitionsAsync().ConfigureAwait(true);
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task SelectSectionAsync(IPopularSectionItemViewModel vm)
    {
        if (vm is null)
        {
            return;
        }

        if (Videos.Count > 0)
        {
            _videoCache[SelectedSection] = Videos.ToList();
        }

        SelectedSection = vm;
        Videos.Clear();
        if (_videoCache.TryGetValue(vm, out var cacheVideos))
        {
            foreach (var video in cacheVideos)
            {
                Videos.Add(video);
            }
        }
        else
        {
            await LoadVideosAsync().ConfigureAwait(true);
        }
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
                await LoadRecommendVideosAsync().ConfigureAwait(true);
            }
            else if (section.Type == PopularSectionType.Hot)
            {
                await LoadHotVideosAsync().ConfigureAwait(true);
            }
            else if (section.Type == PopularSectionType.Rank)
            {
                await LoadTotalRankVideosAsync().ConfigureAwait(true);
            }
        }
        else if (SelectedSection is PopularRankPartitionViewModel partition)
        {
            await LoadPartitionRankVideosAsync(partition.Data).ConfigureAwait(true);
        }

        VideoListUpdated?.Invoke(this, EventArgs.Empty);
        IsVideoLoading = false;
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

        await LoadVideosAsync().ConfigureAwait(true);
    }

    partial void OnNavColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.PopularPageNavColumnWidth, value);
        }
    }

    partial void OnIsNavColumnManualHideChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsPopularPageNavColumnManualHide, value);
        NavColumnWidth = value ? 0 : SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.PopularPageNavColumnWidth, 240d);
    }
}
