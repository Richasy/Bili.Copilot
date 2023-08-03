// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax;
using Microsoft.UI.Xaml.Documents;

namespace Bili.Copilot.Libs.Markdown.Renderers.Blocks;

internal sealed class HeadingRenderer : WinUIObjectRenderer<HeadingBlock>
{
    protected override void Write(WinUIRenderer renderer, HeadingBlock obj)
    {
        var paragraph = new Paragraph();
        var context = renderer.Context;
        switch (obj.Level)
        {
            case 1:
                paragraph.Margin = context.Header1Margin;
                paragraph.FontSize = context.Header1FontSize;
                paragraph.FontWeight = context.Header1FontWeight;
                paragraph.Foreground = context.Header1Foreground;
                break;
            case 2:
                paragraph.Margin = context.Header2Margin;
                paragraph.FontSize = context.Header2FontSize;
                paragraph.FontWeight = context.Header2FontWeight;
                paragraph.Foreground = context.Header2Foreground;
                break;
            case 3:
                paragraph.Margin = context.Header3Margin;
                paragraph.FontSize = context.Header3FontSize;
                paragraph.FontWeight = context.Header3FontWeight;
                paragraph.Foreground = context.Header3Foreground;
                break;
            case 4:
                paragraph.Margin = context.Header4Margin;
                paragraph.FontSize = context.Header4FontSize;
                paragraph.FontWeight = context.Header4FontWeight;
                paragraph.Foreground = context.Header4Foreground;
                break;
            case 5:
                paragraph.Margin = context.Header5Margin;
                paragraph.FontSize = context.Header5FontSize;
                paragraph.FontWeight = context.Header5FontWeight;
                paragraph.Foreground = context.Header5Foreground;
                break;
            case 6:
                paragraph.Margin = context.Header6Margin;
                paragraph.FontSize = context.Header6FontSize;
                paragraph.FontWeight = context.Header6FontWeight;
                paragraph.Foreground = context.Header6Foreground;
                break;
            default:
                break;
        }

        renderer.Add(paragraph);
        renderer.WriteLeafInline(obj);
        renderer.ExtractLastElementAsChild();
    }
}
