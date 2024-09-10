﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using Microsoft.UI;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace BiliCopilot.UI.Controls.Markdown.TextElements;

internal class MyQuote : IAddChild
{
    private Paragraph _paragraph;
    private readonly MyFlowDocument _flowDocument;
    private readonly QuoteBlock _quoteBlock;

    public TextElement TextElement => _paragraph;

    public MyQuote(QuoteBlock quoteBlock)
    {
        _quoteBlock = quoteBlock;
        _paragraph = new Paragraph();

        _flowDocument = new MyFlowDocument(quoteBlock);
        var inlineUIContainer = new InlineUIContainer();

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

        var bar = new Grid
        {
            Width = 4,
            Background = new SolidColorBrush(Colors.Gray),
        };
        bar.SetValue(Grid.ColumnProperty, 0);
        bar.VerticalAlignment = VerticalAlignment.Stretch;
        bar.Margin = new Thickness(0, 0, 4, 0);
        grid.Children.Add(bar);

        var rightGrid = new Grid
        {
            Padding = new Thickness(4),
        };
        rightGrid.Children.Add(_flowDocument.RichTextBlock);

        rightGrid.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(rightGrid);
        grid.Margin = new Thickness(0, 2, 0, 2);

        inlineUIContainer.Child = grid;

        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child) => _flowDocument.AddChild(child);
}
