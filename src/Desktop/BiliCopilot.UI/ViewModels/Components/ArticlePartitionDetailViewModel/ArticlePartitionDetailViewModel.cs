// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 专栏文章分区详情视图模型.
/// </summary>
public sealed partial class ArticlePartitionDetailViewModel : ViewModelBase<PartitionViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionDetailViewModel"/> class.
    /// </summary>
    public ArticlePartitionDetailViewModel(
        PartitionViewModel data,
        IArticleDiscoveryService service)
        : base(data)
    {
        _service = service;
        _logger = this.Get<ILogger<ArticlePartitionDetailViewModel>>();
        _isRecommendPartition = data.Data.Id == "_";
        _isHotPartition = data.Data.Id == "-";
    }

    [RelayCommand]
    private void Initialize()
    {
        if (Articles.Count > 0)
        {
            return;
        }

        if (Data.Children is not null)
        {
            var children = Data.Children.ToList();
            var rcmdPartition = new Partition(Data.Data.Id, ResourceToolkit.GetLocalizedString(StringNames.RecommendArticle));
            children.Insert(0, new PartitionViewModel(rcmdPartition));
            Children = [.. children];
        }

        if (_isRecommendPartition || _isHotPartition)
        {
            IsRecommend = true;
            ChangeChildPartition(Data);
        }
        else
        {
            SortTypes = [.. Enum.GetValues<ArticleSortType>()];
            SelectedSortType = ArticleSortType.Default;
            ChangeChildPartition(Children.First());
        }

        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ChangeSortType(ArticleSortType sortType)
    {
        if (IsRecommend)
        {
            return;
        }

        SelectedSortType = sortType;

        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            _childPartitionArticleCache.Clear();
            Articles.Clear();
            _childPartitionOffsetCache.Clear();
            await LoadArticlesAsync();
        });
    }

    [RelayCommand]
    private void Refresh()
    {
        if (IsRecommend)
        {
            _recommendOffset = 0;
            _recommendCache?.Clear();
            _preventLoadMore = false;
        }
        else
        {
            _childPartitionArticleCache.Remove(CurrentPartition.Data.Id);
            _childPartitionOffsetCache.Remove(CurrentPartition.Data.Id);
        }

        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Articles.Clear();
            await LoadArticlesAsync();
        });
    }

    [RelayCommand]
    private void ChangeChildPartition(PartitionViewModel partition)
    {
        if (partition is null || partition.Data.Equals(CurrentPartition?.Data))
        {
            return;
        }

        IsRecommend = partition.Data.Id == Data.Data.Id;
        if (Articles.Count > 0)
        {
            if (IsRecommend)
            {
                _recommendCache = Articles.ToList();
            }
            else
            {
                _childPartitionArticleCache[CurrentPartition.Data.Id] = Articles.ToList();
            }
        }

        CurrentPartition = partition;

        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Articles.Clear();
            if (IsRecommend && _recommendCache?.Count > 0)
            {
                foreach (var item in _recommendCache)
                {
                    Articles.Add(item);
                }
            }
            else if (_childPartitionArticleCache.TryGetValue(CurrentPartition.Data.Id, out var cache))
            {
                foreach (var item in cache)
                {
                    Articles.Add(item);
                }
            }

            if (Articles.Count == 0)
            {
                await LoadArticlesAsync();
            }
            else
            {
                ArticleListUpdated?.Invoke(this, EventArgs.Empty);
            }
        });
    }

    [RelayCommand]
    private async Task LoadArticlesAsync()
    {
        if (IsArticleLoading || _preventLoadMore)
        {
            return;
        }

        IsArticleLoading = true;
        if (_isRecommendPartition)
        {
            await LoadRecommendPartitionArticlesAsync();
        }
        else if (_isHotPartition)
        {
            await LoadHotPartitionArticlesAsync();
        }
        else if (IsRecommend)
        {
            await LoadRecommendArticlesAsync();
        }
        else
        {
            await LoadChildPartitionArticlesAsync();
        }

        IsArticleLoading = false;
        ArticleListUpdated?.Invoke(this, EventArgs.Empty);
    }

    private async Task LoadRecommendArticlesAsync()
    {
        try
        {
            var (articles, offset) = await _service.GetPartitionArticlesAsync(CurrentPartition.Data, ArticleSortType.Default, _recommendOffset);
            _recommendOffset = offset;
            TryAddArticles(articles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"尝试加载 {Data.Data.Name} 分区推荐文章列表时出错.");
        }
    }

    private async Task LoadChildPartitionArticlesAsync()
    {
        try
        {
            var pn = 0;
            if (_childPartitionOffsetCache.TryGetValue(CurrentPartition.Data.Id, out var offsetCache))
            {
                pn = offsetCache;
            }

            var (articles, nextPn) = await _service.GetPartitionArticlesAsync(CurrentPartition.Data, SelectedSortType, pn);
            _childPartitionOffsetCache[CurrentPartition.Data.Id] = nextPn;
            TryAddArticles(articles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"尝试加载 {Data.Data.Name}/{CurrentPartition.Data.Name} 分区文章时出错.");
        }
    }

    private async Task LoadHotPartitionArticlesAsync()
    {
        try
        {
            // 将榜单所有文章全部加载.
            var categories = await _service.GetHotCategoriesAsync();
            var articles = new List<ArticleInformation>();
            foreach (var cate in categories)
            {
                var d = await _service.GetHotArticlesAsync(cate.Key);
                articles.AddRange(d);
            }

            articles = articles.OrderByDescending(p => p.PublishDateTime).Distinct().ToList();
            TryAddArticles(articles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载热门专栏分区时出错.");
        }

        _preventLoadMore = true;
    }

    private async Task LoadRecommendPartitionArticlesAsync()
    {
        try
        {
            var (articles, _, offset) = await _service.GetRecommendArticlesAsync(_recommendOffset);
            _recommendOffset = offset;
            TryAddArticles(articles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载推荐文章时出错.");
        }
    }

    private void TryAddArticles(IReadOnlyList<ArticleInformation> articles)
    {
        if (articles is not null)
        {
            foreach (var item in articles)
            {
                if (Articles.Any(p => p.Data.Equals(item)))
                {
                    continue;
                }

                Articles.Add(new ArticleItemViewModel(item, ArticleCardStyle.Partition));
            }
        }
    }
}
