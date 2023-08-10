// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Search;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

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

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnSearchBoxSubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, Microsoft.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
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
}

/// <summary>
/// <see cref="SearchBoxModule"/> 的基类.
/// </summary>
public abstract class SearchBoxModuleBase : ReactiveUserControl<SearchBoxViewModel>
{
}
