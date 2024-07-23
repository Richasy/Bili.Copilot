// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Richasy.WinUI.Share.Base;

/// <summary>
/// 应用导航视图项.
/// </summary>
public sealed class AppNavigationViewItem : NavigationViewItem
{
    /// <summary>
    /// <see cref="Symbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(FluentIcons.Common.Symbol), typeof(AppNavigationViewItem), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ShowUnread"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ShowUnreadProperty =
        DependencyProperty.Register(nameof(ShowUnread), typeof(bool), typeof(AppNavigationViewItem), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavigationViewItem"/> class.
    /// </summary>
    public AppNavigationViewItem()
    {
        DefaultStyleKey = typeof(AppNavigationViewItem);
    }

    /// <summary>
    /// 图标.
    /// </summary>
    public FluentIcons.Common.Symbol Symbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// 是否显示未读消息.
    /// </summary>
    public bool ShowUnread
    {
        get => (bool)GetValue(ShowUnreadProperty);
        set => SetValue(ShowUnreadProperty, value);
    }
}

/// <summary>
/// 应用导航视图项显示器.
/// </summary>
public sealed class AppNavigationViewItemPresenter : NavigationViewItemPresenter
{
    /// <summary>
    /// <see cref="Symbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(FluentIcons.Common.Symbol), typeof(AppNavigationViewItemPresenter), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavigationViewItemPresenter"/> class.
    /// </summary>
    public AppNavigationViewItemPresenter()
    {
        DefaultStyleKey = typeof(AppNavigationViewItemPresenter);
    }

    /// <summary>
    /// 图标.
    /// </summary>
    public FluentIcons.Common.Symbol Symbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
}
