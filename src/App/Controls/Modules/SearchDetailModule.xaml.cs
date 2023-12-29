// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 搜索详情模块.
/// </summary>
public sealed partial class SearchDetailModule : SearchDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchDetailModule"/> class.
    /// </summary>
    public SearchDetailModule() => InitializeComponent();

    private void OnNavItemInvokedAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItem as SearchModuleItemViewModel;
        if (item != ViewModel.CurrentModule)
        {
            ViewModel.SelectModuleCommand.Execute(item);
        }
    }

    private void OnFilterItemSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox.DataContext is not SearchFilterViewModel context)
        {
            return;
        }

        var selectItem = comboBox.SelectedItem as Condition;
        if (selectItem != context.CurrentCondition && selectItem != null)
        {
            // 条件变更，重载模块.
            context.CurrentCondition = selectItem;
            ViewModel.ReloadModuleCommand.Execute(default);
        }
    }
}

/// <summary>
/// <see cref="SearchDetailModule"/> 的基类.
/// </summary>
public abstract class SearchDetailModuleBase : ReactiveUserControl<SearchDetailViewModel>
{
}
