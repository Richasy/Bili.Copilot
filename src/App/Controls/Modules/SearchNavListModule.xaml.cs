// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 搜索页导航列表模块.
/// </summary>
public sealed partial class SearchNavListModule : SearchNavListModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchNavListModule"/> class.
    /// </summary>
    public SearchNavListModule() => InitializeComponent();

    private void OnModuleItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement).DataContext as SearchModuleItemViewModel;
        ViewModel.SelectModuleCommand.Execute(context);
    }
}

/// <summary>
/// 搜索页导航列表模块基类.
/// </summary>
public abstract class SearchNavListModuleBase : ReactiveUserControl<SearchDetailViewModel>
{
}
