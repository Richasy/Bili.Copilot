// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 视频视图.
/// </summary>
public sealed partial class VerticalRepeaterView
{
    /// <summary>
    /// <see cref="ItemsSource"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(VerticalRepeaterView), new PropertyMetadata(null));

    /// <summary>
    /// <see cref="ItemTemplate"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(object), typeof(VerticalRepeaterView), new PropertyMetadata(null));

    /// <summary>
    /// <see cref="EnableDetectParentScrollViewer"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty EnableDetectParentScrollViewerProperty =
        DependencyProperty.Register(nameof(EnableDetectParentScrollViewer), typeof(bool), typeof(VerticalRepeaterView), new PropertyMetadata(true));

    /// <summary>
    /// <see cref="VerticalCacheSize"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty VerticalCacheSizeProperty =
        DependencyProperty.Register(nameof(VerticalCacheSize), typeof(int), typeof(VerticalRepeaterView), new PropertyMetadata(4));

    /// <summary>
    /// 在外部的ScrollViewer滚动到接近底部时发生.
    /// </summary>
    public event EventHandler RequestLoadMore;

    /// <summary>
    /// 增量加载请求被触发.
    /// </summary>
    public event EventHandler IncrementalTriggered;

    /// <summary>
    /// 条目模板.
    /// </summary>
    public object ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// 数据源.
    /// </summary>
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 是否启用自动检测父滚动视图.
    /// </summary>
    public bool EnableDetectParentScrollViewer
    {
        get => (bool)GetValue(EnableDetectParentScrollViewerProperty);
        set => SetValue(EnableDetectParentScrollViewerProperty, value);
    }

    /// <summary>
    /// 垂直方向缓存大小.
    /// </summary>
    public int VerticalCacheSize
    {
        get => (int)GetValue(VerticalCacheSizeProperty);
        set => SetValue(VerticalCacheSizeProperty, value);
    }
}
