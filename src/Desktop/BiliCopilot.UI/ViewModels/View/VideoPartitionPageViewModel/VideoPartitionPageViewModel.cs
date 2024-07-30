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
/// 视频分区页面视图模型.
/// </summary>
public sealed partial class VideoPartitionPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPageViewModel"/> class.
    /// </summary>
    public VideoPartitionPageViewModel(
        IVideoDiscoveryService discoveryService,
        ILogger<VideoPartitionPageViewModel> logger)
    {
        _service = discoveryService;
        _logger = logger;

        NavColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.VideoPartitionPageNavColumnWidth, 240d);
        IsNavColumnManualHide = SettingsToolkit.ReadLocalSetting(SettingNames.IsVideoPartitionPageNavColumnManualHide, false);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsPartitionLoading = true;
        var partitions = await _service.GetVideoPartitionsAsync().ConfigureAwait(true);
        if (partitions != null)
        {
            foreach (var item in partitions)
            {
                Partitions.Add(new VideoPartitionViewModel(item));
            }
        }

        IsPartitionLoading = false;
        await Task.Delay(200).ConfigureAwait(true);
        SectionInitialized?.Invoke(this, EventArgs.Empty);
        var lastSelectedPartitionId = SettingsToolkit.ReadLocalSetting(SettingNames.VideoPartitionPageLastSelectedPartitionId, Partitions.First().Data.Id);
        var partition = Partitions.FirstOrDefault(p => p.Data.Id == lastSelectedPartitionId) ?? Partitions.First();
    }

    partial void OnNavColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.VideoPartitionPageNavColumnWidth, value);
        }
    }

    partial void OnIsNavColumnManualHideChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsVideoPartitionPageNavColumnManualHide, value);
        NavColumnWidth = value ? 0 : SettingsToolkit.ReadLocalSetting(SettingNames.VideoPartitionPageNavColumnWidth, 240d);
    }
}
