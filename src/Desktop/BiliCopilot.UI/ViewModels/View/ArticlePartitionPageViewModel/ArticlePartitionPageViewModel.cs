// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 专栏文章分区页面视图模型.
/// </summary>
public sealed partial class ArticlePartitionPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionPageViewModel"/> class.
    /// </summary>
    public ArticlePartitionPageViewModel(
        IArticleDiscoveryService discoveryService,
        ILogger<ArticlePartitionPageViewModel> logger)
    {
        _service = discoveryService;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(ArticlePartitionPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Partitions.Count > 0)
        {
            return;
        }

        IsPartitionLoading = true;
        var hotPartition = new Partition("-", ResourceToolkit.GetLocalizedString(StringNames.HotArticle));
        Partitions.Add(new PartitionViewModel(hotPartition));
        var partitions = await _service.GetPartitionsAsync();
        if (partitions != null)
        {
            foreach (var item in partitions)
            {
                Partitions.Add(new PartitionViewModel(item));
            }
        }

        IsPartitionLoading = false;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            var lastSelectedPartitionId = SettingsToolkit.ReadLocalSetting(SettingNames.ArticlePartitionPageLastSelectedPartitionId, Partitions.First().Data.Id);
            var partition = Partitions.FirstOrDefault(p => p.Data.Id == lastSelectedPartitionId) ?? Partitions.First();
            SelectPartition(partition);
            PartitionInitialized?.Invoke(this, EventArgs.Empty);
        });
    }

    [RelayCommand]
    private void SelectPartition(PartitionViewModel partition)
    {
        if (partition is null || partition.Data.Equals(SelectedPartition?.Data))
        {
            return;
        }

        if (_partitionCache.TryGetValue(partition.Data, out var detail))
        {
            SelectedPartition = detail;
        }
        else
        {
            var vm = new ArticlePartitionDetailViewModel(partition, _service);
            _partitionCache.Add(partition.Data, vm);
            SelectedPartition = vm;
            vm.InitializeCommand.Execute(default);
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.ArticlePartitionPageLastSelectedPartitionId, partition.Data.Id);
    }
}
