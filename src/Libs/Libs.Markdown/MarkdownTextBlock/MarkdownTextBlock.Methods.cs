// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Diagnostics;
using Bili.Copilot.Libs.Markdown.Renderers;
using Markdig;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.Libs.Markdown;

/// <summary>
/// An efficient and extensible control that can parse and render markdown.
/// </summary>
public partial class MarkdownTextBlock
{
    /// <summary>
    /// Called to preform a render of the current Markdown.
    /// </summary>
    private void RenderMarkdown()
    {
        // Leave if we don't have our root yet.
        if (_rootElement == null)
        {
            return;
        }

        // Clear everything that exists.
        _listeningHyperlinks.Clear();

        // Make sure we have something to parse.
        if (string.IsNullOrWhiteSpace(Text))
        {
            _rootElement.Child = null;
        }
        else
        {
            try
            {
                RegisterThemeChangedHandler();
                var theme = _themeListener.CurrentTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
                var context = new RendererContext
                {
                    CurrentTheme = theme,
                    CornerRadius = CornerRadius,
                    UseSyntaxHighlighting = UseSyntaxHighlighting,
                    Background = Background,
                    BorderBrush = BorderBrush,
                    BorderThickness = BorderThickness,
                    CharacterSpacing = CharacterSpacing,
                    FontFamily = FontFamily,
                    FontSize = FontSize,
                    FontStretch = FontStretch,
                    FontStyle = FontStyle,
                    FontWeight = FontWeight,
                    Foreground = Foreground,
                    IsTextSelectionEnabled = IsTextSelectionEnabled,
                    Padding = Padding,
                    CodeStyling = CodeStyling,
                    CodeBackground = CodeBackground,
                    CodeBorderBrush = CodeBorderBrush,
                    CodeBorderThickness = CodeBorderThickness,
                    InlineCodeBorderThickness = InlineCodeBorderThickness,
                    InlineCodeBackground = InlineCodeBackground,
                    InlineCodeBorderBrush = InlineCodeBorderBrush,
                    InlineCodePadding = InlineCodePadding,
                    InlineCodeFontFamily = InlineCodeFontFamily,
                    InlineCodeForeground = InlineCodeForeground,
                    CodeForeground = CodeForeground,
                    CodeFontFamily = CodeFontFamily,
                    CodePadding = CodePadding,
                    CodeMargin = CodeMargin,
                    EmojiFontFamily = EmojiFontFamily,
                    Header1FontSize = Header1FontSize,
                    Header1FontWeight = Header1FontWeight,
                    Header1Margin = Header1Margin,
                    Header1Foreground = Header1Foreground,
                    Header2FontSize = Header2FontSize,
                    Header2FontWeight = Header2FontWeight,
                    Header2Margin = Header2Margin,
                    Header2Foreground = Header2Foreground,
                    Header3FontSize = Header3FontSize,
                    Header3FontWeight = Header3FontWeight,
                    Header3Margin = Header3Margin,
                    Header3Foreground = Header3Foreground,
                    Header4FontSize = Header4FontSize,
                    Header4FontWeight = Header4FontWeight,
                    Header4Margin = Header4Margin,
                    Header4Foreground = Header4Foreground,
                    Header5FontSize = Header5FontSize,
                    Header5FontWeight = Header5FontWeight,
                    Header5Margin = Header5Margin,
                    Header5Foreground = Header5Foreground,
                    Header6FontSize = Header6FontSize,
                    Header6FontWeight = Header6FontWeight,
                    Header6Margin = Header6Margin,
                    Header6Foreground = Header6Foreground,
                    HorizontalRuleBrush = HorizontalRuleBrush,
                    HorizontalRuleMargin = HorizontalRuleMargin,
                    HorizontalRuleThickness = HorizontalRuleThickness,
                    ListMargin = ListMargin,
                    ListGutterWidth = ListGutterWidth,
                    ListBulletSpacing = ListBulletSpacing,
                    ParagraphMargin = ParagraphMargin,
                    ParagraphLineHeight = ParagraphLineHeight,
                    QuoteBackground = QuoteBackground,
                    QuoteBorderBrush = QuoteBorderBrush,
                    QuoteBorderThickness = QuoteBorderThickness,
                    QuoteForeground = QuoteForeground,
                    QuoteMargin = QuoteMargin,
                    QuotePadding = QuotePadding,
                    TableBorderBrush = TableBorderBrush,
                    TableBorderThickness = TableBorderThickness,
                    TableCellPadding = TableCellPadding,
                    TableMargin = TableMargin,
                    TextWrapping = TextWrapping,
                    LinkForeground = LinkForeground,
                    ImageStretch = ImageStretch,
                    ImageMaxHeight = ImageMaxHeight,
                    ImageMaxWidth = ImageMaxWidth,
                    WrapCodeBlock = WrapCodeBlock,
                    FlowDirection = FlowDirection,
                };

                var pipeline = new MarkdownPipelineBuilder()
                    .UseAutoLinks()
                    .UseEmojiAndSmiley()
                    .UseEmphasisExtras()
                    .UseGridTables()
                    .UsePipeTables()
                    .Build();
                var renderer = new WinUIRenderer();
                renderer.Context = context;
                pipeline.Setup(renderer);
                var doc = Markdig.Markdown.Parse(Text, pipeline);
                _rootElement.Child = renderer.Render(doc) as UIElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while parsing and rendering: " + ex.Message);
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }
    }
}
