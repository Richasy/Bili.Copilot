// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 热门导航列表模块.
/// </summary>
public sealed partial class PopularNavListModule : PopularNavListModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularNavListModule"/> class.
    /// </summary>
    public PopularNavListModule()
    {
        InitializeComponent();
        ViewModel = PopularPageViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.FirstLoadCommand.Execute(default);

    private void OnRecommendItemClick(object sender, RoutedEventArgs e)
        => ViewModel.ChangeModuleTypeCommand.Execute(Models.Constants.App.PopularType.Recommend);

    private void OnRankItemClick(object sender, RoutedEventArgs e)
        => ViewModel.ChangeModuleTypeCommand.Execute(Models.Constants.App.PopularType.Rank);

    private void OnHotItemClick(object sender, RoutedEventArgs e)
        => ViewModel.ChangeModuleTypeCommand.Execute(Models.Constants.App.PopularType.Hot);

    private void OnPartitionClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as PartitionItem).ViewModel;
        ViewModel.OpenPartitionCommand.Execute(item);
    }
}

/// <summary>
/// 热门导航列表模块基类.
/// </summary>
public abstract class PopularNavListModuleBase : ReactiveUserControl<PopularPageViewModel>
{
}
