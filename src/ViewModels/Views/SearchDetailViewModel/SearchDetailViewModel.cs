// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchDetailViewModel : InformationFlowViewModel<SearchModuleItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchDetailViewModel"/> class.
    /// </summary>
    public SearchDetailViewModel()
    {
        _requestStatusCache = new Dictionary<SearchModuleType, bool>();
        _filters = new Dictionary<SearchModuleType, IEnumerable<SearchFilterViewModel>>();

        Videos = new ObservableCollection<VideoItemViewModel>();
        Animes = new ObservableCollection<SeasonItemViewModel>();
        Movies = new ObservableCollection<SeasonItemViewModel>();
        Users = new ObservableCollection<UserItemViewModel>();
        Articles = new ObservableCollection<ArticleItemViewModel>();
        Lives = new ObservableCollection<LiveItemViewModel>();
        CurrentFilters = new ObservableCollection<SearchFilterViewModel>();
        Modules = new ObservableCollection<SearchModuleItemViewModel>();

        InitializeSearchModules();
        AttachIsRunningToAsyncCommand(p => IsReloadingModule = p, ReloadModuleCommand, SelectModuleCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadModuleCommand, SelectModuleCommand);
    }

    /// <summary>
    /// 设置搜索关键字.
    /// </summary>
    /// <param name="keyword">关键词.</param>
    public void SetKeyword(string keyword)
    {
        Keyword = keyword;
        BeforeReload();
        SelectModuleCommand.Execute(Modules.First());
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _requestStatusCache.Clear();
        _filters.Clear();
        TryClear(Videos);
        TryClear(Animes);
        TryClear(Articles);
        TryClear(Movies);
        TryClear(Users);
        TryClear(Lives);
        SearchProvider.Instance.ClearStatus();
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        var moduleType = CurrentModule.Type;
        if (CurrentFilters.Count == 0)
        {
            if (!_filters.ContainsKey(moduleType))
            {
                await InitializeFiltersAsync(moduleType);
            }

            var filters = _filters[moduleType];
            filters.ToList().ForEach(p => CurrentFilters.Add(p));
        }

        IsCurrentFilterEmpty = CurrentFilters.Count == 0;

        if (_requestStatusCache.TryGetValue(moduleType, out var isEnd) && isEnd)
        {
            return;
        }

        await RequestDataAsync();
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestSearchResultFailed)}\n{errorMsg}";

    private static SearchModuleItemViewModel GetModuleItemViewModel(SearchModuleType type, string title, bool isEnabled = true)
        => new(type, title, isEnabled);

    [RelayCommand]
    private async Task SelectModuleAsync(SearchModuleItemViewModel vm)
    {
        ClearException();
        TryClear(CurrentFilters);
        CurrentModule = vm;
        foreach (var item in Modules)
        {
            item.IsSelected = item.Equals(vm);
        }

        ClearCurrentModule();
        await GetDataAsync();
    }

    [RelayCommand]
    private async Task ReloadModuleAsync()
    {
        if (_isRequesting)
        {
            return;
        }

        try
        {
            _isRequesting = true;
            ClearException();
            ClearCurrentModule();
            await RequestDataAsync();
        }
        catch (System.Exception)
        {
            throw;
        }

        _isRequesting = false;
    }

    private void InitializeSearchModules()
    {
        Modules.Add(GetModuleItemViewModel(SearchModuleType.Video, ResourceToolkit.GetLocalizedString(StringNames.Video)));
        Modules.Add(GetModuleItemViewModel(SearchModuleType.Anime, ResourceToolkit.GetLocalizedString(StringNames.Anime)));
        Modules.Add(GetModuleItemViewModel(SearchModuleType.Live, ResourceToolkit.GetLocalizedString(StringNames.Live)));
        Modules.Add(GetModuleItemViewModel(SearchModuleType.User, ResourceToolkit.GetLocalizedString(StringNames.User)));
        Modules.Add(GetModuleItemViewModel(SearchModuleType.Movie, ResourceToolkit.GetLocalizedString(StringNames.Movie)));
        Modules.Add(GetModuleItemViewModel(SearchModuleType.Article, ResourceToolkit.GetLocalizedString(StringNames.SpecialColumn)));
    }

    private void ClearCurrentModule()
    {
        switch (CurrentModule.Type)
        {
            case SearchModuleType.Video:
                TryClear(Videos);
                SearchProvider.Instance.ResetComprehensiveStatus();
                break;
            case SearchModuleType.Anime:
                TryClear(Animes);
                SearchProvider.Instance.ResetAnimeStatus();
                break;
            case SearchModuleType.Live:
                TryClear(Lives);
                SearchProvider.Instance.ResetLiveStatus();
                break;
            case SearchModuleType.User:
                TryClear(Users);
                SearchProvider.Instance.ResetUserStatus();
                break;
            case SearchModuleType.Movie:
                TryClear(Movies);
                SearchProvider.Instance.ResetMovieStatus();
                break;
            case SearchModuleType.Article:
                TryClear(Articles);
                SearchProvider.Instance.ResetArticleStatus();
                break;
            default:
                break;
        }
    }

    private void CheckModuleContentEmpty()
    {
        IsCurrentContentEmpty = CurrentModule.Type switch
        {
            SearchModuleType.Video => Videos.Count == 0,
            SearchModuleType.Anime => Animes.Count == 0,
            SearchModuleType.Live => Lives.Count == 0,
            SearchModuleType.User => Users.Count == 0,
            SearchModuleType.Movie => Movies.Count == 0,
            SearchModuleType.Article => Articles.Count == 0,
            _ => true,
        };
    }

    private void CheckModuleVisibility()
    {
        IsVideoModuleShown = CurrentModule.Type == SearchModuleType.Video;
        IsAnimeModuleShown = CurrentModule.Type == SearchModuleType.Anime;
        IsMovieModuleShown = CurrentModule.Type == SearchModuleType.Movie;
        IsArticleModuleShown = CurrentModule.Type == SearchModuleType.Article;
        IsLiveModuleShown = CurrentModule.Type == SearchModuleType.Live;
        IsUserModuleShown = CurrentModule.Type == SearchModuleType.User;
    }

    partial void OnCurrentModuleChanged(SearchModuleItemViewModel value)
    {
        if (value != null)
        {
            CheckModuleVisibility();
        }
    }
}
