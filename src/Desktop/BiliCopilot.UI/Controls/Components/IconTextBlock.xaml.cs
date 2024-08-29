// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Media;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 图标文本块.
/// </summary>
public sealed partial class IconTextBlock : UserControl
{
    /// <summary>
    /// <see cref="Symbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(FluentIcons.Common.Symbol), typeof(IconTextBlock), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(IconTextBlock), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IconBrush"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconBrushProperty =
        DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(IconTextBlock), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="IconTextBlock"/> class.
    /// </summary>
    public IconTextBlock() => InitializeComponent();

    /// <summary>
    /// 图标.
    /// </summary>
    public FluentIcons.Common.Symbol Symbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// 文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 图标画刷.
    /// </summary>
    public Brush IconBrush
    {
        get => (Brush)GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }
}
