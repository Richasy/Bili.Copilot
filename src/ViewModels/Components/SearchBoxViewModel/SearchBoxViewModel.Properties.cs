// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Threading;
using Bili.Copilot.Models.Data.Search;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索框视图模型.
/// </summary>
public sealed partial class SearchBoxViewModel
{
    private readonly DispatcherTimer _suggestionTimer;

    private CancellationTokenSource _suggestionCancellationTokenSource;
    private bool _isKeywordChanged;
    private bool _isInitialized;

    [ObservableProperty]
    private string _queryText;

    [ObservableProperty]
    private bool _isInitializing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static SearchBoxViewModel Instance { get; } = new();

    /// <summary>
    /// 热搜集合.
    /// </summary>
    public ObservableCollection<SearchSuggest> HotSearchCollection { get; }

    /// <summary>
    /// 搜索建议集合.
    /// </summary>
    public ObservableCollection<SearchSuggest> AutoSuggestCollection { get; }
}
