// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;
using System.Windows.Input;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 点赞按钮.
/// </summary>
public sealed partial class LikeButton : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsLiked"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsLikedProperty =
        DependencyProperty.Register(nameof(IsLiked), typeof(bool), typeof(LikeButton), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Tip"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TipProperty =
        DependencyProperty.Register(nameof(LikeButton), typeof(string), typeof(LikeButton), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Command"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(LikeButton), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="TripleCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TripleCommandProperty =
        DependencyProperty.Register(nameof(TripleCommand), typeof(ICommand), typeof(LikeButton), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="LikeButton"/> class.
    /// </summary>
    public LikeButton() => InitializeComponent();

    /// <summary>
    /// 是否已点赞.
    /// </summary>
    public bool IsLiked
    {
        get => (bool)GetValue(IsLikedProperty);
        set => SetValue(IsLikedProperty, value);
    }

    /// <summary>
    /// 提示文本.
    /// </summary>
    public string Tip
    {
        get => (string)GetValue(TipProperty);
        set => SetValue(TipProperty, value);
    }

    /// <summary>
    /// 命令.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// 一键三连命令.
    /// </summary>
    public ICommand TripleCommand
    {
        get => (ICommand)GetValue(TripleCommandProperty);
        set => SetValue(TripleCommandProperty, value);
    }

    private void OnLikeButtonClick(object sender, RoutedEventArgs e)
    {
        if (!IsLiked)
        {
            TripleTip.IsOpen = true;
        }
    }

    private void OnTripleButtonClick(TeachingTip sender, object args)
        => TripleTip.IsOpen = false;
}
