// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Search;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPageViewModel"/> class.
    /// </summary>
    public SearchPageViewModel(
        ISearchService service,
        ILogger<SearchPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(SearchPage);

    [RelayCommand]
    private async Task SearchAsync(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            throw new ArgumentNullException(nameof(keyword));
        }

        if (keyword == Keyword)
        {
            return;
        }

        Keyword = keyword;
        Sections = default;
        SelectedSection = default;

        try
        {
            IsSearching = true;
            var (videos, partitions, nextVideoOffset) = await _service.GetComprehensiveSearchResultAsync(keyword);
            var sections = new List<ISearchSectionDetailViewModel>()
            {
                CreateVideoSection(videos, nextVideoOffset),
            };
            if (partitions is not null)
            {
                foreach (var section in partitions)
                {
                    var sec = CreateSection(section);
                    if (sec is not null)
                    {
                        sections.Add(sec);
                    }
                }
            }

            Sections = [.. sections];
            SelectSection(Sections.FirstOrDefault());
            SectionInitialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取搜索结果时失败.");
        }
        finally
        {
            IsSearching = false;
        }
    }

    /// <summary>
    /// 选择分区.
    /// </summary>
    [RelayCommand]
    private void SelectSection(ISearchSectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        section.TryFirstLoadCommand.Execute(default);
    }

    private VideoSearchSectionDetailViewModel CreateVideoSection(IReadOnlyList<VideoInformation> videos, string offset)
    {
        var newSection = new VideoSearchSectionDetailViewModel(_service);
        newSection.Initialize(Keyword, new SearchPartition(0, ResourceToolkit.GetLocalizedString(StringNames.Video)));
        newSection.SetFirstPageData(videos, offset);
        return newSection;
    }

    private ISearchSectionDetailViewModel CreateSection(SearchPartition partition)
    {
        var type = (SearchSectionType)partition.Id;
        var sectionDetail = type switch
        {
            SearchSectionType.Anime or SearchSectionType.Cinema => (ISearchSectionDetailViewModel)new PgcSearchSectionDetailViewModel(_service),
            SearchSectionType.Live => new LiveSearchSectionDetailViewModel(_service),
            SearchSectionType.User => new UserSearchSectionDetailViewModel(_service),
            SearchSectionType.Article => new ArticleSearchSectionDetailViewModel(_service),
            _ => default
        };

        sectionDetail?.Initialize(Keyword, partition);
        return sectionDetail;
    }
}
