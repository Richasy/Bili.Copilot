// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索模块条目视图模型.
/// </summary>
public sealed partial class SearchModuleItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private SearchModuleType _type;

    [ObservableProperty]
    private FluentSymbol _symbol;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchModuleItemViewModel"/> class.
    /// </summary>
    public SearchModuleItemViewModel(SearchModuleType type, string title, bool isEnabled = true)
    {
        Type = type;
        Title = title;
        IsEnabled = isEnabled;
        Symbol = type switch
        {
            SearchModuleType.Video => FluentSymbol.VideoClip,
            SearchModuleType.Anime => FluentSymbol.Dust,
            SearchModuleType.Movie => FluentSymbol.FilmstripPlay,
            SearchModuleType.Article => FluentSymbol.DocumentBulletList,
            SearchModuleType.Live => FluentSymbol.Video,
            SearchModuleType.User => FluentSymbol.People,
            _ => FluentSymbol.Globe
        };
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SearchModuleItemViewModel model && Type == model.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => Type.GetHashCode();
}
