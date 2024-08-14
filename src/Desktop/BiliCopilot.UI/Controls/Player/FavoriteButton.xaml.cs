// Copyright (c) Bili Copilot. All rights reserved.

using System.Windows.Input;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 收藏按钮.
/// </summary>
public sealed partial class FavoriteButton : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsFavorited"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsFavoritedProperty =
        DependencyProperty.Register(nameof(IsFavorited), typeof(bool), typeof(CoinButton), new PropertyMetadata(default));

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
    /// Initializes a new instance of the <see cref="FavoriteButton"/> class.
    /// </summary>
    public FavoriteButton() => InitializeComponent();

    /// <summary>
    /// 是否已收藏.
    /// </summary>
    public bool IsFavorited
    {
        get => (bool)GetValue(IsFavoritedProperty);
        set => SetValue(IsFavoritedProperty, value);
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

    private void OnFavoriteButtonClick(object sender, RoutedEventArgs e)
    {
        if (IsFavorited)
        {
            return;
        }

        FavoriteTip.IsOpen = true;
    }

    private void OnChecked(object sender, RoutedEventArgs e)
    {
        if (!IsFavorited)
        {
            Btn.IsChecked = false;
        }
    }

    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (IsFavorited)
        {
            Btn.IsChecked = true;
        }
    }

    private void OnConfirmButtonClick(TeachingTip sender, object args)
    {
        _ = this;
    }
}
