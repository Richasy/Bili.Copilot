﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace BiliCopilot.UI.Controls.Markdown.TextElements;

internal class MyTableUIElement : Panel
{
    private readonly int _columnCount;
    private readonly int _rowCount;
    private readonly double _borderThickness;
    private double[] _columnWidths;
    private double[] _rowHeights;

    public MyTableUIElement(int columnCount, int rowCount, double borderThickness, Brush borderBrush)
    {
        _columnCount = columnCount;
        _rowCount = rowCount;
        _borderThickness = borderThickness;
        for (var col = 0; col < columnCount + 1; col++)
        {
            Children.Add(new Rectangle { Fill = borderBrush });
        }

        for (var row = 0; row < rowCount + 1; row++)
        {
            Children.Add(new Rectangle { Fill = borderBrush });
        }
    }

    // Helper method to enumerate FrameworkElements instead of UIElements.
    private IEnumerable<FrameworkElement> ContentChildren
    {
        get
        {
            for (var i = _columnCount + _rowCount + 2; i < Children.Count; i++)
            {
                yield return (FrameworkElement)Children[i];
            }
        }
    }

    // Helper method to get table vertical edges.
    private IEnumerable<Rectangle> VerticalLines
    {
        get
        {
            for (var i = 0; i < _columnCount + 1; i++)
            {
                yield return (Rectangle)Children[i];
            }
        }
    }

    // Helper method to get table horizontal edges.
    private IEnumerable<Rectangle> HorizontalLines
    {
        get
        {
            for (var i = _columnCount + 1; i < _columnCount + _rowCount + 2; i++)
            {
                yield return (Rectangle)Children[i];
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // Measure the width of each column, with no horizontal width restrictions.
        var naturalColumnWidths = new double[_columnCount];
        foreach (var child in ContentChildren)
        {
            var columnIndex = Grid.GetColumn(child);
            child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            naturalColumnWidths[columnIndex] = Math.Max(naturalColumnWidths[columnIndex], child.DesiredSize.Width);
        }

        // Now figure out the actual column widths.
        var remainingContentWidth = availableSize.Width - ((_columnCount + 1) * _borderThickness);
        _columnWidths = new double[_columnCount];
        var remainingColumnCount = _columnCount;
        while (remainingColumnCount > 0)
        {
            // Calculate the fair width of all columns.
            var fairWidth = Math.Max(0, remainingContentWidth / remainingColumnCount);

            // Are there any columns less than that?  If so, they get what they are asking for.
            var recalculationNeeded = false;
            for (var i = 0; i < _columnCount; i++)
            {
                if (_columnWidths[i] == 0 && naturalColumnWidths[i] < fairWidth)
                {
                    _columnWidths[i] = naturalColumnWidths[i];
                    remainingColumnCount--;
                    remainingContentWidth -= _columnWidths[i];
                    recalculationNeeded = true;
                }
            }

            // If there are no columns less than the fair width, every remaining column gets that width.
            if (recalculationNeeded == false)
            {
                for (var i = 0; i < _columnCount; i++)
                {
                    if (_columnWidths[i] == 0)
                    {
                        _columnWidths[i] = fairWidth;
                    }
                }

                break;
            }
        }

        // TODO: we can skip this step if none of the column widths changed, and just re-use
        // the row heights we obtained earlier.

        // Now measure row heights.
        _rowHeights = new double[_rowCount];
        foreach (var child in ContentChildren)
        {
            var columnIndex = Grid.GetColumn(child);
            var rowIndex = Grid.GetRow(child);
            child.Measure(new Size(_columnWidths[columnIndex], double.PositiveInfinity));
            _rowHeights[rowIndex] = Math.Max(_rowHeights[rowIndex], child.DesiredSize.Height);
        }

        return new Size(
            _columnWidths.Sum() + (_borderThickness * (_columnCount + 1)),
            _rowHeights.Sum() + ((_rowCount + 1) * _borderThickness));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_columnWidths == null || _rowHeights == null)
        {
            throw new InvalidOperationException("Expected Measure to be called first.");
        }

        // Arrange content.
        foreach (var child in ContentChildren)
        {
            var columnIndex = Grid.GetColumn(child);
            var rowIndex = Grid.GetRow(child);

            var rect = new Rect(_borderThickness, 0, 0, 0);

            for (var col = 0; col < columnIndex; col++)
            {
                rect.X += _borderThickness + _columnWidths[col];
            }

            rect.Y = _borderThickness;
            for (var row = 0; row < rowIndex; row++)
            {
                rect.Y += _borderThickness + _rowHeights[row];
            }

            rect.Width = _columnWidths[columnIndex];
            rect.Height = _rowHeights[rowIndex];
            child.Arrange(rect);
        }

        // Arrange vertical border elements.
        {
            var colIndex = 0;
            double x = 0;
            foreach (var borderLine in VerticalLines)
            {
                borderLine.Arrange(new Rect(x, 0, _borderThickness, finalSize.Height));
                if (colIndex >= _columnWidths.Length)
                {
                    break;
                }

                x += _borderThickness + _columnWidths[colIndex];
                colIndex++;
            }
        }

        // Arrange horizontal border elements.
        {
            var rowIndex = 0;
            double y = 0;
            foreach (var borderLine in HorizontalLines)
            {
                borderLine.Arrange(new Rect(0, y, finalSize.Width, _borderThickness));
                if (rowIndex >= _rowHeights.Length)
                {
                    break;
                }

                y += _borderThickness + _rowHeights[rowIndex];
                rowIndex++;
            }
        }

        return finalSize;
    }
}
