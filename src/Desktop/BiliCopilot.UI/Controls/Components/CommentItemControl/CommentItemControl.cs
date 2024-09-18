// Copyright (c) Bili Copilot. All rights reserved.

using System.Windows.Input;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 评论项控件.
/// </summary>
public sealed partial class CommentItemControl : LayoutControlBase<CommentItemViewModel>
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

    private Button _userButton;
    private ButtonBase _moreButton;
    private ButtonBase _replyButton;

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

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _userButton = GetTemplateChild("UserButton") as Button;
        _moreButton = GetTemplateChild("MoreButton") as ButtonBase;
        _replyButton = GetTemplateChild("ReplyButton") as ButtonBase;
        if (ViewModel is not null)
        {
            if (_userButton is not null)
            {
                _userButton.Command = ViewModel.ShowUserSpaceCommand;
            }

            if (_moreButton is not null)
            {
                _moreButton.Command = ShowMoreCommand;
            }

            if (_replyButton is not null)
            {
                _replyButton.Command = ViewModel.MarkReplyTargetCommand;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(CommentItemViewModel? oldValue, CommentItemViewModel? newValue)
    {
        if (_userButton is not null)
        {
            _userButton.Command = newValue?.ShowUserSpaceCommand;
        }

        if (_replyButton is not null)
        {
            _replyButton.Command = newValue?.MarkReplyTargetCommand;
        }

        if (_moreButton is not null)
        {
            _moreButton.Command = ShowMoreCommand;
        }
    }
}
