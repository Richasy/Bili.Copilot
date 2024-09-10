﻿// Copyright (c) Rodel. All rights reserved.
// <auto-generated />

using Markdig.Extensions.TaskLists;
using Microsoft.UI;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Media3D;

namespace BiliCopilot.UI.Controls.Markdown.TextElements;

internal class MyTaskListCheckBox : IAddChild
{
    private readonly TaskList _taskList;
    public TextElement TextElement { get; private set; }

    public MyTaskListCheckBox(TaskList taskList)
    {
        _taskList = taskList;
        var grid = new Grid();
        var transform = new CompositeTransform3D
        {
            TranslateY = 2,
        };
        grid.Transform3D = transform;
        grid.Width = 16;
        grid.Height = 16;
        grid.Margin = new Thickness(2, 0, 2, 0);
        grid.BorderThickness = new Thickness(1);
        grid.BorderBrush = new SolidColorBrush(Colors.Gray);
        var icon = new FontIcon
        {
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Glyph = "\uE73E",
        };
        grid.Children.Add(taskList.Checked ? icon : new TextBlock());
        grid.Padding = new Thickness(0);
        grid.CornerRadius = new CornerRadius(2);
        var inlineUIContainer = new InlineUIContainer
        {
            Child = grid,
        };
        TextElement = inlineUIContainer;
    }

    public void AddChild(IAddChild child)
    {
    }
}
