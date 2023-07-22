// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using Bili.Copilot.Libs.Flyleaf.Controls;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Vortice.DXGI;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器.
/// </summary>
public sealed class BiliPlayer : ContentControl, IMediaTransportControls
{
    /// <summary>
    /// <see cref="Player"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty PlayerProperty =
        DependencyProperty.Register(nameof(Player), typeof(Player), typeof(BiliPlayer), new PropertyMetadata(default, new PropertyChangedCallback(OnPlayerChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayer"/> class.
    /// </summary>
    public BiliPlayer()
    {
        DefaultStyleKey = typeof(BiliPlayer);
    }

    /// <summary>
    /// 图像播放面板.
    /// </summary>
    public SwapChainPanel Panel { get; set; }

    /// <summary>
    /// 播放器.
    /// </summary>
    public Player Player
    {
        get => (Player)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    /// <inheritdoc/>
    public bool Player_CanHideCursor()
        => Player_GetFullScreen();

    /// <inheritdoc/>
    public void Player_Disposed() => Player = null;

    /// <inheritdoc/>
    public bool Player_GetFullScreen()
        => Window.Current.AppWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen;

    /// <inheritdoc/>
    public void Player_SetFullScreen(bool value)
    {
        if (value)
        {
            Window.Current.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
        else
        {
            Window.Current.AppWindow.SetPresenter(AppWindowPresenterKind.Default);
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        if (GetTemplateChild("SwapChainPanel") is SwapChainPanel panel)
        {
            Panel = panel;
            Panel.SizeChanged += OnPanelSizeChanged;
        }

        if (Player != null)
        {
            ReplacePlayer(default);
        }
    }

    private static void OnPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BiliPlayer instance && e.NewValue is Player player)
        {
            instance.ReplacePlayer(e.OldValue as Player);
        }
    }

    private void OnPanelSizeChanged(object sender, SizeChangedEventArgs e)
        => Player?.renderer.ResizeBuffers((int)e.NewSize.Width, (int)e.NewSize.Height);

    private void ReplacePlayer(Player oldPlayer = default)
    {
        if (oldPlayer != null)
        {
            Debug.WriteLine($"取消链接播放器 {oldPlayer.PlayerId}");
            oldPlayer.VideoDecoder.DestroySwapChain();
            oldPlayer.Host = default;
        }

        if (Player == null)
        {
            return;
        }

        Player.Host?.Player_Disposed();
        if (Player == null)
        {
            return;
        }

        Debug.WriteLine($"链接播放器 {Player.PlayerId}");
        Player.Host = this;
        Background = new SolidColorBrush(new() { A = Player.Config.Video.BackgroundColor.A, R = Player.Config.Video.BackgroundColor.R, G = Player.Config.Video.BackgroundColor.G, B = Player.Config.Video.BackgroundColor.B });
        Player.VideoDecoder.CreateSwapChain(SwapChainClbk);
    }

    private void SwapChainClbk(IDXGISwapChain2 swapChain)
    {
        using (var nativeObject = SharpGen.Runtime.ComObject.As<Vortice.WinUI.ISwapChainPanelNative2>(Panel))
        {
            nativeObject.SetSwapChain(swapChain);
        }

        Player?.renderer.ResizeBuffers((int)ActualWidth, (int)ActualHeight);
    }
}
