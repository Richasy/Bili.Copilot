// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Models.Data.Search;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索框视图模型.
/// </summary>
public sealed partial class SearchBoxViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchBoxViewModel"/> class.
    /// </summary>
    private SearchBoxViewModel()
    {
        HotSearchCollection = new ObservableCollection<SearchSuggest>();
        AutoSuggestCollection = new ObservableCollection<SearchSuggest>();

        _suggestionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(350),
        };
        _suggestionTimer.Tick += OnSuggestionTimerTickAsync;

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, InitializeCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized || IsInitializing)
        {
            return;
        }

        var data = await SearchProvider.GetHotSearchListAsync();
        data.ToList().ForEach(HotSearchCollection.Add);
        _isInitialized = true;
    }

    [RelayCommand]
    private void SearchBySuggest(SearchSuggest suggest)
        => SearchByText(suggest.Keyword);

    [RelayCommand]
    private void SearchByText(string text)
    {
        QueryText = text;
        AppViewModel.Instance.SearchContentCommand.Execute(text);
    }

    private void InitializeSuggestionCancellationTokenSource()
    {
        if (_suggestionCancellationTokenSource != null
            && !_suggestionCancellationTokenSource.IsCancellationRequested)
        {
            _suggestionCancellationTokenSource.Cancel();
            _suggestionCancellationTokenSource = null;
        }

        _suggestionCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
    }

    private async Task LoadSearchSuggestionAsync()
    {
        if (string.IsNullOrEmpty(QueryText))
        {
            return;
        }

        TryClear(AutoSuggestCollection);
        try
        {
            var suggestion = await SearchProvider.GetSearchSuggestionAsync(QueryText, _suggestionCancellationTokenSource.Token);
            if (suggestion == null)
            {
                return;
            }

            suggestion.ToList().ForEach(AutoSuggestCollection.Add);
        }
        catch (TaskCanceledException)
        {
            // 任务中止表示有新的搜索请求或者请求超时，这是预期的错误，不予处理.
        }
        catch (Exception ex)
        {
            LogException(ex);
        }
    }

    private async void OnSuggestionTimerTickAsync(object sender, object e)
    {
        if (_isKeywordChanged)
        {
            _isKeywordChanged = false;
            InitializeSuggestionCancellationTokenSource();
            await LoadSearchSuggestionAsync();
        }
    }

    partial void OnQueryTextChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            // 搜索关键词为空，表示用户或应用清除了内容，此时不进行请求，并重置状态.
            _isKeywordChanged = false;
            _suggestionTimer.Stop();
            InitializeSuggestionCancellationTokenSource();
            TryClear(AutoSuggestCollection);
        }
        else
        {
            _isKeywordChanged = true;
            if (!_suggestionTimer.IsEnabled)
            {
                _suggestionTimer.Start();
            }
        }
    }
}
