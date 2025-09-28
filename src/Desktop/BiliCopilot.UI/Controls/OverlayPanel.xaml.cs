// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// �������.
/// </summary>
public sealed partial class OverlayPanel : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsShow"/> ����������.
    /// </summary>
    public static readonly DependencyProperty IsShowProperty =
        DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(OverlayPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Element"/> ����������.
    /// </summary>
    public static readonly DependencyProperty ElementProperty =
        DependencyProperty.Register(nameof(Element), typeof(object), typeof(OverlayPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="PanelAlignment"/> ����������.
    /// </summary>
    public static readonly DependencyProperty PanelAlignmentProperty =
        DependencyProperty.Register(nameof(PanelAlignment), typeof(HorizontalAlignment), typeof(OverlayPanel), new PropertyMetadata(HorizontalAlignment.Left));

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayPanel"/> class.
    /// </summary>
    public OverlayPanel() => InitializeComponent();

    /// <summary>
    /// �Ƿ���ʾ.
    /// </summary>
    public bool IsShow
    {
        get => (bool)GetValue(IsShowProperty);
        set => SetValue(IsShowProperty, value);
    }

    /// <summary>
    /// �ڲ�Ԫ��.
    /// </summary>
    public object Element
    {
        get => (object)GetValue(ElementProperty);
        set => SetValue(ElementProperty, value);
    }

    /// <summary>
    /// ���λ��.
    /// </summary>
    public HorizontalAlignment PanelAlignment
    {
        get => (HorizontalAlignment)GetValue(PanelAlignmentProperty);
        set => SetValue(PanelAlignmentProperty, value);
    }

    private void OnHolderTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => IsShow = !IsShow;
}