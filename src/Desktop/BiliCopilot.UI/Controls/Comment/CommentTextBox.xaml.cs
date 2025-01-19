// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.Base;
using System.Windows.Input;

namespace BiliCopilot.UI.Controls.Comment;

/// <summary>
/// 评论输入框.
/// </summary>
public sealed partial class CommentTextBox : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(CommentTextBox), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="SendCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SendCommandProperty =
        DependencyProperty.Register(nameof(SendCommand), typeof(IAsyncRelayCommand), typeof(CommentTextBox), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ReplyTip"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ReplyTipProperty =
        DependencyProperty.Register(nameof(ReplyTip), typeof(string), typeof(CommentTextBox), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ResetSelectedCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ResetSelectedCommandProperty =
        DependencyProperty.Register(nameof(ResetSelectedCommand), typeof(ICommand), typeof(CommentTextBox), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentTextBox"/> class.
    /// </summary>
    public CommentTextBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 回复文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 发送评论命令.
    /// </summary>
    public IAsyncRelayCommand SendCommand
    {
        get => (IAsyncRelayCommand)GetValue(SendCommandProperty);
        set => SetValue(SendCommandProperty, value);
    }

    /// <summary>
    /// 评论提示.
    /// </summary>
    public string ReplyTip
    {
        get => (string)GetValue(ReplyTipProperty);
        set => SetValue(ReplyTipProperty, value);
    }

    /// <summary>
    /// 重置已选择评论的命令.
    /// </summary>
    public ICommand ResetSelectedCommand
    {
        get => (ICommand)GetValue(ResetSelectedCommandProperty);
        set => SetValue(ResetSelectedCommandProperty, value);
    }

    private async void OnReplySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        var finalText = string.IsNullOrEmpty(ReplyTip)
            ? Text
            : $"回复 @{ReplyTip} : {Text}";
        await SendCommand.ExecuteAsync(finalText);
        Text = string.Empty;
    }

    private void OnItemClick(object sender, string e)
        => Text += e;

    private void OnFlyoutClosed(object sender, object e)
        => ReplyBox.Focus(FocusState.Programmatic);
}
