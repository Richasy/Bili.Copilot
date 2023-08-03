// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;

namespace Bili.Copilot.Libs.Markdown;

/// <summary>
/// An efficient and extensible control that can parse and render markdown.
/// </summary>
public partial class MarkdownTextBlock
{
    /// <summary>
    /// Calls OnPropertyChanged.
    /// </summary>
    private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as MarkdownTextBlock;

        // Defer to the instance method.
        instance?.OnPropertyChanged(d, e.Property);
    }

    /// <summary>
    /// Fired when the value of a DependencyProperty is changed.
    /// </summary>
    private void OnPropertyChanged(DependencyObject d, DependencyProperty prop) => RenderMarkdown();
}
