// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Search;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 搜索建议项视图模型.
/// </summary>
public sealed partial class SearchSuggestItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchContent;

    [ObservableProperty]
    private string? _regionName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSuggestItemViewModel"/> class.
    /// </summary>
    public SearchSuggestItemViewModel(SearchSuggestItemBase item)
    {
        Keyword = item.Keyword;
        SearchContent = item.Text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSuggestItemViewModel"/> class.
    /// </summary>
    public SearchSuggestItemViewModel(string regionId, string regionName)
    {
        RegionId = regionId;
        RegionName = regionName;
    }

    /// <summary>
    /// 关键词.
    /// </summary>
    public string Keyword { get; set; }

    /// <summary>
    /// 区域标识.
    /// </summary>
    public string? RegionId { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SearchSuggestItemViewModel model && Keyword == model.Keyword && RegionId == model.RegionId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Keyword, RegionId);
}
