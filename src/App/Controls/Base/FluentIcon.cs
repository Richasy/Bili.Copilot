// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// Fluent 图标.
/// </summary>
public sealed class FluentIcon : FontIcon
{
    /// <summary>
    /// Dependency property for <see cref="Symbol"/>.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(FluentSymbol), typeof(FluentIcon), new PropertyMetadata(default, OnSymbolChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentIcon"/> class.
    /// </summary>
    public FluentIcon()
        => FontFamily = new FontFamily("/Assets/FluentSystemIcon.ttf#FluentSystemIcons-Resizable");

    /// <summary>
    /// Icon.
    /// </summary>
    public FluentSymbol Symbol
    {
        get => (FluentSymbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is FluentSymbol symbol)
        {
            var icon = d as FluentIcon;
            icon.Glyph = ((char)symbol).ToString();
        }
    }
}
