// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.DynamicPageViewModel;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 动态页面的 UP 主列表模块.
/// </summary>
public sealed partial class DynamicUpListModule : DynamicAllModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicUpListModule"/> class.
    /// </summary>
    public DynamicUpListModule()
    {
        InitializeComponent();
        ViewModel = DynamicPageViewModel.Instance;
    }

    private void OnAllDynamicItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsAllDynamicSelected = true;

        if (ViewModel.SelectedUp != null)
        {
            ViewModel.SelectedUp.IsSelected = false;
            ViewModel.SelectedUp = default;
        }
    }

    private void OnUpItemClick(object sender, RoutedEventArgs e)
    {
        var user = (sender as FrameworkElement).DataContext as UserItemViewModel;
        ViewModel.SelectUpCommand.Execute(user);
    }
}

/// <summary>
/// <see cref="DynamicUpListModule"/> 的基类.
/// </summary>
public abstract class DynamicAllModuleBase : ReactiveUserControl<DynamicPageViewModel>
{
}
