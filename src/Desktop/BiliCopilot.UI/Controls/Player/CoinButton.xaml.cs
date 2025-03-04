// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;
using System.Windows.Input;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 投币按钮.
/// </summary>
public sealed partial class CoinButton : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsCoined"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsCoinedProperty =
        DependencyProperty.Register(nameof(IsCoined), typeof(bool), typeof(CoinButton), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Tip"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TipProperty =
        DependencyProperty.Register(nameof(CoinButton), typeof(string), typeof(CoinButton), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Command"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CoinButton), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="AlsoLike"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AlsoLikeProperty =
        DependencyProperty.Register(nameof(AlsoLike), typeof(bool), typeof(CoinButton), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="CoinButton"/> class.
    /// </summary>
    public CoinButton() => InitializeComponent();

    /// <summary>
    /// 是否已投币.
    /// </summary>
    public bool IsCoined
    {
        get => (bool)GetValue(IsCoinedProperty);
        set => SetValue(IsCoinedProperty, value);
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
    /// 同时点赞.
    /// </summary>
    public bool AlsoLike
    {
        get => (bool)GetValue(AlsoLikeProperty);
        set => SetValue(AlsoLikeProperty, value);
    }

    private void OnCoinButtonClick(object sender, RoutedEventArgs e)
    {
        if (IsCoined)
        {
            return;
        }

        CoinTip.IsOpen = true;
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
        if (!IsCoined)
        {
            Btn.IsChecked = false;
        }
    }

    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (IsCoined)
        {
            Btn.IsChecked = true;
        }
    }

    private void OnOneCoinButtonClick(TeachingTip sender, object args)
    {
        Command?.Execute(1);
        CoinTip.IsOpen = false;
    }

    private void OnTwoCoinButtonClick(TeachingTip sender, object args)
    {
        Command?.Execute(2);
        CoinTip.IsOpen = false;
    }
}
