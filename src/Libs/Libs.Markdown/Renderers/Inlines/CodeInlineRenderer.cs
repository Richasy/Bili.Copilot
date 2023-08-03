// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax.Inlines;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace Bili.Copilot.Libs.Markdown.Renderers.Inlines;

internal sealed class CodeInlineRenderer : WinUIObjectRenderer<CodeInline>
{
    protected override void Write(WinUIRenderer renderer, CodeInline obj)
    {
        var context = renderer.Context;
        var block = renderer.CreateDefaultTextBlock();
        block.Text = obj.Content;
        block.FontFamily = context.InlineCodeFontFamily ?? context.FontFamily;
        block.Foreground = context.InlineCodeForeground ?? context.Foreground;
        block.FontSize = context.FontSize;

        var border = new Border
        {
            BorderThickness = context.InlineCodeBorderThickness,
            BorderBrush = context.InlineCodeBorderBrush,
            Background = context.InlineCodeBackground,
            Padding = context.InlineCodePadding,
            Margin = context.InlineCodeMargin,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
            Child = block,
            CornerRadius = context.CornerRadius,
            RenderTransform = new TranslateTransform { Y = 4 },
        };

        var inlineUIContainer = new InlineUIContainer { Child = border };
        renderer.Add(inlineUIContainer);
    }
}
