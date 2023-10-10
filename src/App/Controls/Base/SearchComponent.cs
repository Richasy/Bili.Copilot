// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 搜索组件.
/// </summary>
public abstract class SearchComponent : ReactiveUserControl<ViewModelBase>
{
    /// <summary>
    /// <see cref="ItemsSource"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(SearchComponent), new PropertyMetadata(default));

    /// <summary>
    /// 数据源.
    /// </summary>
    public object ItemsSource
    {
        get => (object)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 增量请求事件回调.
    /// </summary>
    protected void OnIncrementalTriggered(object sender, System.EventArgs e)
    {
        if (DataContext is SearchDetailViewModel viewModel)
        {
            viewModel.IncrementalCommand.Execute(default);
        }
    }
}
