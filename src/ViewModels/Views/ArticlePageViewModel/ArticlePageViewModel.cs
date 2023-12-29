// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 专栏文章页面的视图模型.
/// </summary>
public sealed partial class ArticlePageViewModel : InformationFlowViewModel<ArticleItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePageViewModel"/> class.
    /// </summary>
    private ArticlePageViewModel()
    {
        _caches = new Dictionary<PartitionItemViewModel, IEnumerable<ArticleInformation>>();
        NavListColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ArticleNavListColumnWidth, 280d);
        Partitions = new ObservableCollection<PartitionItemViewModel>();
        SortTypes = new ObservableCollection<ArticleSortType>()
            {
                ArticleSortType.Default,
                ArticleSortType.Newest,
                ArticleSortType.Read,
                ArticleSortType.Reply,
                ArticleSortType.Like,
                ArticleSortType.Favorite,
            };

        SortType = ArticleSortType.Default;
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        if (CurrentPartition != null)
        {
            ArticleProvider.Instance.ResetPartitionStatus(CurrentPartition.Data.Id);
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestArticleFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (Partitions.Count == 0)
        {
            var partitions = await ArticleProvider.GetPartitionsAsync();
            partitions.ToList().ForEach(p => Partitions.Add(new PartitionItemViewModel(p)));
            CurrentPartition = Partitions.FirstOrDefault();
        }

        var partition = CurrentPartition;
        var data = await ArticleProvider.Instance.GetPartitionArticlesAsync(partition.Data.Id, SortType);

        if (data.Articles?.Count() > 0)
        {
            foreach (var article in data.Articles)
            {
                if (Items.Any(p => p.Data.Equals(article)))
                {
                    continue;
                }

                var articleVM = new ArticleItemViewModel(article);
                Items.Add(articleVM);
            }

            var articles = Items
                    .Select(p => p.Data)
                    .ToList();
            _caches[CurrentPartition] = articles;
        }
    }

    [RelayCommand]
    private void SelectPartition(PartitionItemViewModel partition)
    {
        TryClear(Items);
        CurrentPartition = partition;
        RequestScrollToTop?.Invoke(this, EventArgs.Empty);
        if (_caches.ContainsKey(partition))
        {
            var items = _caches[partition];
            foreach (var data in items)
            {
                var articleVM = new ArticleItemViewModel(data);
                Items.Add(articleVM);
            }
        }
        else
        {
            _ = InitializeCommand.ExecuteAsync(default);
        }
    }

    partial void OnCurrentPartitionChanged(PartitionItemViewModel value)
    {
        IsRecommendPartition = value?.Data.Id == "0";
    }

    partial void OnNavListColumnWidthChanged(double value)
    {
        if (value >= 240)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.ArticleNavListColumnWidth, value);
        }
    }
}
