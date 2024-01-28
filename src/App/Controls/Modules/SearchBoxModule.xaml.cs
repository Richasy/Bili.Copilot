// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Search;
using Bili.Copilot.ViewModels;
using CommunityToolkit.WinUI;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 搜索框模块.
/// </summary>
public sealed partial class SearchBoxModule : SearchBoxModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchBoxModule"/> class.
    /// </summary>
    public SearchBoxModule()
    {
        InitializeComponent();
        ViewModel = SearchBoxViewModel.Instance;
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 设置焦点.
    /// </summary>
    public async void SetFocusAsync()
    {
        var textBox = AppSearchBox.FindDescendant<TextBox>();
        if (textBox is not null)
        {
            await FocusManager.TryFocusAsync(textBox, FocusState.Programmatic);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnSearchBoxSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is SearchSuggest ss)
        {
            ViewModel.SearchBySuggestCommand.Execute(ss);
        }

        if (!string.IsNullOrEmpty(sender.Text))
        {
            ViewModel.SearchByTextCommand.Execute(sender.Text);
        }
    }

    private void OnHotSearchItemClick(object sender, EventArgs e)
        => HotSearchFlyout.Hide();
}

/// <summary>
/// <see cref="SearchBoxModule"/> 的基类.
/// </summary>
public abstract class SearchBoxModuleBase : ReactiveUserControl<SearchBoxViewModel>
{
}
