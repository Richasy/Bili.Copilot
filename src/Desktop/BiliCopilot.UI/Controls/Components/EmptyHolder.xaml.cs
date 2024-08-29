// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 空白占位符.
/// </summary>
public sealed partial class EmptyHolder : UserControl
{
    /// <summary>
    /// Dependency property of <see cref="Title"/>.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(EmptyHolder),
            new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="Description"/>.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(EmptyHolder),
            new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="Emoji"/>.
    /// </summary>
    public static readonly DependencyProperty EmojiProperty =
        DependencyProperty.Register(
            nameof(Emoji),
            typeof(string),
            typeof(EmptyHolder),
            new PropertyMetadata(default));

    /// <summary>
    /// Dependency property of <see cref="ActionElement"/>.
    /// </summary>
    public static readonly DependencyProperty ActionElementProperty =
        DependencyProperty.Register(
            nameof(ActionElement),
            typeof(object),
            typeof(EmptyHolder),
            new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyHolder"/> class.
    /// </summary>
    public EmptyHolder() => InitializeComponent();

    /// <summary>
    /// Title.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Description.
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Emoji.
    /// </summary>
    public string Emoji
    {
        get => (string)GetValue(EmojiProperty);
        set => SetValue(EmojiProperty, value);
    }

    /// <summary>
    /// The element that performs the action.
    /// </summary>
    public object ActionElement
    {
        get => GetValue(ActionElementProperty);
        set => SetValue(ActionElementProperty, value);
    }
}
