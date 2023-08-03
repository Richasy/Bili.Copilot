// Copyright (c) Bili Copilot. All rights reserved.

using Markdig.Syntax;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.Libs.Markdown.Renderers.Blocks;

internal sealed class ListRenderer : WinUIObjectRenderer<ListBlock>
{
    protected override void Write(WinUIRenderer renderer, ListBlock obj)
    {
        var context = renderer.Context;
        var grid = new Grid
        {
            Margin = context.ListMargin,
        };

        grid.ColumnSpacing = context.ListBulletSpacing;
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(context.ListGutterWidth) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        for (var i = 0; i < obj.Count; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        renderer.Add(grid);

        var startIndex = string.IsNullOrEmpty(obj.OrderedStart)
                    ? string.IsNullOrEmpty(obj.DefaultOrderedStart)
                        ? 1
                        : int.Parse(obj.DefaultOrderedStart)
                    : int.Parse(obj.OrderedStart);
        for (var i = 0; i < obj.Count; i++)
        {
            var sign = obj.IsOrdered
                ? $"{startIndex + i}{obj.OrderedDelimiter}"
                : $"{obj.BulletType}";
            var bulletBlock = renderer.CreateDefaultTextBlock();
            bulletBlock.Text = sign;
            bulletBlock.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(bulletBlock, i);
            renderer.Add(bulletBlock);
            renderer.ExtractLastElementAsChild();

            var listItemBlock = (ListItemBlock)obj[i];
            var content = new StackPanel();
            Grid.SetColumn(content, 1);
            Grid.SetRow(content, i);
            renderer.Add(content);
            renderer.WriteChildren(listItemBlock);
            renderer.ExtractLastElementAsChild();
        }

        renderer.ExtractLastElementAsChild();
    }
}
