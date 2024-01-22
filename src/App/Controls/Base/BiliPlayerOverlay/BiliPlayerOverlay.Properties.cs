﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.App.Other;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public partial class BiliPlayerOverlay
{
    /// <summary>
    /// <see cref="MediaPresenter"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty MediaPresenterProperty =
        DependencyProperty.Register(nameof(MediaPresenter), typeof(object), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IsLive"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsLiveProperty =
        DependencyProperty.Register(nameof(IsLive), typeof(bool), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="SectionHeaderItemsSource"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SectionHeaderItemsSourceProperty =
        DependencyProperty.Register(nameof(SectionHeaderItemsSource), typeof(object), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="SectionContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SectionContentProperty =
        DependencyProperty.Register(nameof(SectionContent), typeof(object), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="SectionHeaderSelectedItem"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SectionHeaderSelectedItemProperty =
        DependencyProperty.Register(nameof(SectionHeaderSelectedItem), typeof(object), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// 区块标头被点击.
    /// </summary>
    public event EventHandler<PlayerSectionHeader> SectionHeaderItemInvoked;

    /// <summary>
    /// 面板状态切换.
    /// </summary>
    public event EventHandler<double> PaneToggled;

    /// <summary>
    /// 光标是否停留在覆盖层上.
    /// </summary>
    public bool IsPointerStay { get; set; }

    /// <summary>
    /// 播放信息展示.
    /// </summary>
    public object MediaPresenter
    {
        get => GetValue(MediaPresenterProperty);
        set => SetValue(MediaPresenterProperty, value);
    }

    /// <summary>
    /// 是否为直播.
    /// </summary>
    public bool IsLive
    {
        get => (bool)GetValue(IsLiveProperty);
        set => SetValue(IsLiveProperty, value);
    }

    /// <summary>
    /// 侧面板的头部数据源.
    /// </summary>
    public object SectionHeaderItemsSource
    {
        get => GetValue(SectionHeaderItemsSourceProperty);
        set => SetValue(SectionHeaderItemsSourceProperty, value);
    }

    /// <summary>
    /// 侧面板头部选中条目.
    /// </summary>
    public object SectionHeaderSelectedItem
    {
        get => GetValue(SectionHeaderSelectedItemProperty);
        set => SetValue(SectionHeaderSelectedItemProperty, value);
    }

    /// <summary>
    /// 侧面板的内容.
    /// </summary>
    public object SectionContent
    {
        get => GetValue(SectionContentProperty);
        set => SetValue(SectionContentProperty, value);
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
