// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;
using System.Windows.Input;

namespace BiliCopilot.UI.Controls.Comment;

/// <summary>
/// 评论控件.
/// </summary>
public sealed partial class CommentItemControl : CommentItemControlBase
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
    public CommentItemControl()
        => InitializeComponent();

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

/// <summary>
/// Comment item control base.
/// </summary>
public abstract class CommentItemControlBase : LayoutUserControlBase<CommentItemViewModel>;
