﻿// Copyright (c) Reader Copilot. All rights reserved.
// <auto-generated />

using Markdig.Extensions.Tables;
using Microsoft.UI.Xaml.Documents;

namespace Bili.Copilot.App.Controls.Markdown.TextElements;

internal class MyTableRow : IAddChild
{
    private readonly TableRow _tableRow;
    private readonly StackPanel _stackPanel;
    private Paragraph _paragraph;

    public TextElement TextElement => _paragraph;

    public MyTableRow(TableRow tableRow)
    {
        _tableRow = tableRow;
        _paragraph = new Paragraph();

        _stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };
        var inlineUIContainer = new InlineUIContainer
        {
            Child = _stackPanel,
        };
        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child)
    {
        if (child is MyTableCell cellChild)
        {
            var richTextBlock = new RichTextBlock();
            richTextBlock.Blocks.Add((Paragraph)cellChild.TextElement);
            _stackPanel.Children.Add(richTextBlock);
        }
    }
}
