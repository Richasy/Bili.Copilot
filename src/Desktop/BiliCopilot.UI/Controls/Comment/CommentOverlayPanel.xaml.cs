// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Comment;

/// <summary>
/// 文章阅读器评论面板.
/// </summary>
public sealed partial class CommentOverlayPanel : CommentMainPanelBase
{
    /// <summary>
    /// <see cref="IsPanelOpened"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsPanelOpenedProperty =
        DependencyProperty.Register(nameof(IsPanelOpened), typeof(bool), typeof(CommentOverlayPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentOverlayPanel"/> class.
    /// </summary>
    public CommentOverlayPanel() => InitializeComponent();

    /// <summary>
    /// 面板可见性.
    /// </summary>
    public bool IsPanelOpened
    {
        get => (bool)GetValue(IsPanelOpenedProperty);
        set => SetValue(IsPanelOpenedProperty, value);
    }

    private void OnHolderTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => IsPanelOpened = !IsPanelOpened;
}
