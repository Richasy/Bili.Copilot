// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Markdig.Extensions.Tables;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.Libs.Markdown.Renderers.Blocks;

internal sealed class TableBlockRenderer : WinUIObjectRenderer<Table>
{
    protected override void Write(WinUIRenderer renderer, Table obj)
    {
        var context = renderer.Context;
        var table = new MarkdownTable(obj.ColumnDefinitions.Count - 1, obj.Count, context.TableBorderThickness, context.TableBorderBrush)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = context.TableMargin,
        };

        renderer.Add(table);
        for (var rowIndex = 0; rowIndex < obj.Count; rowIndex++)
        {
            var row = (TableRow)obj[rowIndex];

            // Add each cell.
            for (var cellIndex = 0; cellIndex < Math.Min(obj.ColumnDefinitions.Count, row.Count); cellIndex++)
            {
                var cell = (TableCell)row[cellIndex];

                // Cell content.
                var cellContent = renderer.CreateDefaultRichTextBlock();
                cellContent.Margin = context.TableCellPadding;
                Grid.SetRow(cellContent, rowIndex);
                Grid.SetColumn(cellContent, cellIndex);
                Grid.SetColumnSpan(cellContent, cell.ColumnSpan);
                renderer.Add(cellContent);
                switch (obj.ColumnDefinitions[cellIndex].Alignment)
                {
                    case TableColumnAlign.Left:
                        cellContent.TextAlignment = TextAlignment.Left;
                        break;
                    case TableColumnAlign.Center:
                        cellContent.TextAlignment = TextAlignment.Center;
                        break;
                    case TableColumnAlign.Right:
                        cellContent.TextAlignment = TextAlignment.Right;
                        break;
                }

                if (rowIndex == 0)
                {
                    cellContent.FontWeight = FontWeights.Bold;
                }

                renderer.Write(cell);
            }
        }

        renderer.ExtractLastElementAsChild();
    }
}
