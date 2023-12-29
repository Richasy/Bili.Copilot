// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 动画导航列表模块.
/// </summary>
public sealed partial class AnimeNavListModule : AnimeNavListModuleBase
{
    private readonly BangumiRecommendDetailViewModel _bangumi;
    private readonly TimelineViewModel _timeline;
    private readonly DomesticRecommendDetailViewModel _domestic;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeNavListModule"/> class.
    /// </summary>
    public AnimeNavListModule()
    {
        InitializeComponent();
        ViewModel = AnimePageViewModel.Instance;
        _bangumi = BangumiRecommendDetailViewModel.Instance;
        _timeline = TimelineViewModel.Instance;
        _domestic = DomesticRecommendDetailViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => AnimeTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;

    private void OnAnimeTypeSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.CurrentType = (AnimeDisplayType)AnimeTypeSelection.SelectedIndex;

    private void OnTimelineItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as TimelineItemViewModel;
        _timeline.OpenTimelineCommand.Execute(data);
    }

    private void OnBangumiConditionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox.DataContext is IndexFilterItemViewModel source
            && comboBox.SelectedItem is Condition item)
        {
            var index = source.Data.Conditions.ToList().IndexOf(item);
            if (index >= 0 && index != source.SelectedIndex)
            {
                source.SelectedIndex = index;
                _ = _bangumi.ReloadCommand.ExecuteAsync(null);
            }
        }
    }

    private void OnDomesticConditionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox.DataContext is IndexFilterItemViewModel source
                && comboBox.SelectedItem is Condition item)
        {
            var index = source.Data.Conditions.ToList().IndexOf(item);
            if (index >= 0 && index != source.SelectedIndex)
            {
                source.SelectedIndex = index;
                _ = _domestic.ReloadCommand.ExecuteAsync(null);
            }
        }
    }
}

/// <summary>
/// 动画导航列表模块基类.
/// </summary>
public abstract class AnimeNavListModuleBase : ReactiveUserControl<AnimePageViewModel>
{
}
