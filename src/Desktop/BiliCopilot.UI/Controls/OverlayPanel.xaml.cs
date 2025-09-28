// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 浮动面板.
/// </summary>
public sealed partial class OverlayPanel : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsShow"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsShowProperty =
        DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(OverlayPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Element"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ElementProperty =
        DependencyProperty.Register(nameof(Element), typeof(object), typeof(OverlayPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="PanelAlignment"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty PanelAlignmentProperty =
        DependencyProperty.Register(nameof(PanelAlignment), typeof(HorizontalAlignment), typeof(OverlayPanel), new PropertyMetadata(HorizontalAlignment.Left));

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayPanel"/> class.
    /// </summary>
    public OverlayPanel() => InitializeComponent();

    /// <summary>
    /// 是否显示.
    /// </summary>
    public bool IsShow
    {
        get => (bool)GetValue(IsShowProperty);
        set => SetValue(IsShowProperty, value);
    }

    /// <summary>
    /// 内部元素.
    /// </summary>
    public object Element
    {
        get => (object)GetValue(ElementProperty);
        set => SetValue(ElementProperty, value);
    }

    /// <summary>
    /// 面板位置.
    /// </summary>
    public HorizontalAlignment PanelAlignment
    {
        get => (HorizontalAlignment)GetValue(PanelAlignmentProperty);
        set => SetValue(PanelAlignmentProperty, value);
    }

    private void OnHolderTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => IsShow = !IsShow;
}