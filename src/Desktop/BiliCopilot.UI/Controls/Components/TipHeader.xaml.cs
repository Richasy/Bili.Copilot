// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 带注释的头部控件.
/// </summary>
public sealed partial class TipHeader : UserControl
{
    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(TipHeader), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Comment"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommentProperty =
        DependencyProperty.Register(nameof(Comment), typeof(string), typeof(TipHeader), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="TipHeader"/> class.
    /// </summary>
    public TipHeader() => InitializeComponent();

    /// <summary>
    /// 获取或设置标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 获取或设置注释.
    /// </summary>
    public string Comment
    {
        get => (string)GetValue(CommentProperty);
        set => SetValue(CommentProperty, value);
    }
}
