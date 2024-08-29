// Copyright (c) Bili Copilot. All rights reserved.

using System.Windows.Input;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 评论项控件.
/// </summary>
public sealed class CommentItemControl : LayoutControlBase<CommentItemViewModel>
{
    /// <summary>
    /// <see cref="ShowMoreCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ShowMoreCommandProperty =
        DependencyProperty.Register(nameof(ShowMoreCommand), typeof(ICommand), typeof(CommentItemControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IsMoreEnabled"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsMoreEnabledProperty =
        DependencyProperty.Register(nameof(IsMoreEnabled), typeof(bool), typeof(CommentItemControl), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentItemControl"/> class.
    /// </summary>
    public CommentItemControl() => DefaultStyleKey = typeof(CommentItemControl);

    /// <summary>
    /// 显示更多命令.
    /// </summary>
    public ICommand ShowMoreCommand
    {
        get => (ICommand)GetValue(ShowMoreCommandProperty);
        set => SetValue(ShowMoreCommandProperty, value);
    }

    /// <summary>
    /// 是否启用更多.
    /// </summary>
    public bool IsMoreEnabled
    {
        get => (bool)GetValue(IsMoreEnabledProperty);
        set => SetValue(IsMoreEnabledProperty, value);
    }
}
