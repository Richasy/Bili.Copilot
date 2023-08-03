// Copyright (c) Bili Copilot. All rights reserved.

using ColorCode;
using CommunityToolkit.WinUI.UI;
using Markdig.Syntax;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Bili.Copilot.Libs.Markdown.Renderers.Blocks;

internal sealed class CodeBlockRenderer : WinUIObjectRenderer<CodeBlock>
{
    protected override void Write(WinUIRenderer renderer, CodeBlock obj)
    {
        var context = renderer.Context;
        var viewer = new ScrollViewer
        {
            Background = context.CodeBackground,
            BorderBrush = context.CodeBorderBrush,
            BorderThickness = context.CodeBorderThickness,
            Padding = context.CodePadding,
            Margin = context.CodeMargin,
            CornerRadius = context.CornerRadius,
            HorizontalScrollMode = ScrollMode.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
        };

        if (!context.WrapCodeBlock)
        {
            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            viewer.HorizontalScrollMode = ScrollMode.Auto;
            viewer.VerticalScrollMode = ScrollMode.Disabled;
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        renderer.Add(viewer);
        var rtb = new RichTextBlock
        {
            FontFamily = context.CodeFontFamily ?? context.FontFamily,
            Foreground = context.CodeForeground ?? context.Foreground,
            LineHeight = context.FontSize * 1.4,
            FlowDirection = context.FlowDirection,
        };

        rtb.PointerWheelChanged += (sender, args) =>
        {
            var pointerPoint = args.GetCurrentPoint((UIElement)sender);

            if (pointerPoint.Properties.IsHorizontalMouseWheel)
            {
                args.Handled = false;
                return;
            }

            var rootViewer = renderer.GetRootElement().FindAscendant<ScrollViewer>();
            if (rootViewer != null)
            {
                RendererContext.PointerWheelChanged?.Invoke(rootViewer, new object[] { args });
                args.Handled = true;
            }
        };

        renderer.Add(rtb);

        // Begin code parsing
        var paragraph = new Paragraph();
        renderer.Add(paragraph);
        var needSyntax = false;
        if (obj is FencedCodeBlock fcb
            && !string.IsNullOrEmpty(fcb.Info)
            && context.UseSyntaxHighlighting)
        {
            var language = Languages.FindById(fcb.Info);
            if (language != null)
            {
                needSyntax = true;
                var formatter = context.CodeStyling != null
                    ? new RichTextBlockFormatter(context.CodeStyling)
                    : new RichTextBlockFormatter(context.CurrentTheme);

                formatter.FormatInlines(WinUIRenderer.GetRawText(fcb), language, paragraph.Inlines);
            }
        }

        if (!needSyntax)
        {
            renderer.WriteLeafRawLines(obj);
        }

        // Add paragraph to RichTextBlock.
        renderer.ExtractLastElementAsChild();

        // Add RichTextBlock to ScrollViewer.
        renderer.ExtractLastElementAsChild();

        // Add ScrollViewer to rootElement.
        renderer.ExtractLastElementAsChild();
    }
}
