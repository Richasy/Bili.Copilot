// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 顶部导航栏条目.
/// </summary>
public sealed partial class TopNavigationItem : LayoutUserControlBase
{
    /// <summary>
    /// 标识 <see cref="IsSelected"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(TopNavigationItem), new PropertyMetadata(default));

    /// <summary>
    /// 标识 <see cref="Symbol"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(FluentIcons.Common.Symbol), typeof(TopNavigationItem), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="TopNavigationItem"/> class.
    /// </summary>
    public TopNavigationItem() => InitializeComponent();

    /// <summary>
    /// 条目点击时发生.
    /// </summary>
    public event EventHandler Click;

    /// <summary>
    /// 获取或设置是否选中.
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// 图标.
    /// </summary>
    public FluentIcons.Common.Symbol Symbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        if (IsSelected)
        {
            return;
        }

        Click?.Invoke(this, EventArgs.Empty);
    }
}
