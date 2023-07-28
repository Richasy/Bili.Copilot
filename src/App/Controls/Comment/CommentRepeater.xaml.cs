// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Comment;

/// <summary>
/// 评论列表.
/// </summary>
public sealed partial class CommentRepeater : UserControl
{
    /// <summary>
    /// <see cref="ItemsSource"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(CommentRepeater), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="TopComment"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TopCommentProperty =
        DependencyProperty.Register(nameof(TopComment), typeof(CommentItemViewModel), typeof(CommentRepeater), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRepeater"/> class.
    /// </summary>
    public CommentRepeater() => InitializeComponent();

    /// <summary>
    /// 数据源.
    /// </summary>
    public object ItemsSource
    {
        get => (object)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 置顶评论.
    /// </summary>
    public CommentItemViewModel TopComment
    {
        get => (CommentItemViewModel)GetValue(TopCommentProperty);
        set => SetValue(TopCommentProperty, value);
    }

    private void OnIncrementalTriggered(object sender, System.EventArgs e)
        => (DataContext as InformationFlowViewModel<CommentItemViewModel>)?.IncrementalCommand.Execute(default);
}
