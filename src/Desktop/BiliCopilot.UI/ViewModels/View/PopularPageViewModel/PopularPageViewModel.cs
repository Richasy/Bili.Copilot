// Copyright (c) Bili Copilot. All rights reserved.

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
        await LoadPartitionsAsync().ConfigureAwait(true);
        SelectSectionCommand.Execute(Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task SelectSectionAsync(IPopularSectionItemViewModel vm)
    {
        if (vm is null)
        {
            return;
        }

        SelectedSection = vm;
        if (Videos.Count > 0)
        {
            _videoCache[SelectedSection] = Videos.ToList();
        }

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
            if (IsVideoLoading)
            {
                return;
            }

            IsVideoLoading = true;
            if (vm is PopularSectionItemViewModel section)
            {
                if (section.Type == PopularSectionType.Recommend)
                {
                    await LoadRecommendVideosAsync().ConfigureAwait(true);
                }
            }

            IsVideoLoading = false;
        }
    }

    private async Task LoadRecommendVideosAsync()
    {
        try
        {
            var (videos, nextOffset) = await _service.GetRecommendVideoListAsync().ConfigureAwait(true);
            _recommendOffset = nextOffset;
            if (videos is not null)
            {
                foreach (var item in videos)
                {
                    Videos.Add(new VideoItemViewModel(item));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载推荐视频时出错.");
        }
    }

    private async Task LoadPartitionsAsync()
    {
        IsPartitionLoading = true;
        var partitions = await _service.GetVideoPartitionsAsync().ConfigureAwait(true);
        if (partitions != null)
        {
            foreach (var item in partitions)
            {
                Sections.Add(new PopularRankPartitionViewModel(item));
            }
        }

        IsPartitionLoading = false;
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
