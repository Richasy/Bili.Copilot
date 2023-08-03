// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.Libs.Markdown.Renderers.Blocks;

internal sealed class QuoteBlockRenderer : WinUIObjectRenderer<QuoteBlock>
{
    protected override void Write(WinUIRenderer renderer, QuoteBlock obj)
    {
        var context = renderer.Context;
        var border = new Border
        {
            Margin = context.QuoteMargin,
            Background = context.QuoteBackground,
            BorderBrush = context.QuoteBorderBrush,
            BorderThickness = context.QuoteBorderThickness,
            Padding = context.QuotePadding,
            CornerRadius = context.CornerRadius,
        };

        renderer.Add(border);
        var panel = new StackPanel();
        renderer.Add(panel);
        renderer.WriteChildren(obj);
        renderer.ExtractLastElementAsChild();
        renderer.ExtractLastElementAsChild();
    }
}
