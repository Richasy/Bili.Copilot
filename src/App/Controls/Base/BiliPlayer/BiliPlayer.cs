// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using System.Diagnostics;
using Bili.Copilot.Libs.Flyleaf.Controls;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Media;
using Vortice.DXGI;
using Windows.Media.Playback;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器.
/// </summary>
public sealed class BiliPlayer : BiliPlayerBase, IMediaTransportControls
{
    /// <summary>
    /// <see cref="Overlay"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty OverlayProperty =
        DependencyProperty.Register(nameof(Overlay), typeof(object), typeof(BiliPlayer), new PropertyMetadata(default, new PropertyChangedCallback(OnOverlayChanged)));

    private MediaPlayerElement _mediaElement;
    private SwapChainPanel _swapChainPanel;
    private Grid _rootGrid;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayer"/> class.
    /// </summary>
    public BiliPlayer()
    {
        DefaultStyleKey = typeof(BiliPlayer);
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 覆盖层.
    /// </summary>
    public object Overlay
    {
        get => GetValue(OverlayProperty);
        set => SetValue(OverlayProperty, value);
    }

    /// <inheritdoc/>
    public bool Player_CanHideCursor()
        => Player_GetFullScreen();

    /// <inheritdoc/>
    public void Player_Disposed()
    {
    }

    /// <inheritdoc/>
    public bool Player_GetFullScreen() => true;

    /// <inheritdoc/>
    public void Player_SetFullScreen(bool value)
    {
        return;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IPlayerViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is IPlayerViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChanged;
        }

        ReloadPlayer();
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        if (GetTemplateChild("SwapChainPanel") is SwapChainPanel panel)
        {
            _swapChainPanel = panel;
            _swapChainPanel.SizeChanged += OnPanelSizeChanged;
        }

        if (GetTemplateChild("MediaElement") is MediaPlayerElement element)
        {
            _mediaElement = element;
        }

        if (GetTemplateChild("RootGrid") is Grid grid)
        {
            _rootGrid = grid;
        }

        if (ViewModel?.Player != null)
        {
            ReloadPlayer();
        }
    }

    private static void OnOverlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is BiliPlayerOverlay overlay)
        {
            var instance = d as BiliPlayer;
            overlay.PaneToggled += instance.OnOverlayPaneToggled;
        }
    }

    private void OnOverlayPaneToggled(object sender, double e)
    {
        if (_rootGrid == null)
        {
            return;
        }

        _rootGrid.Padding = new Thickness(e, 0, 0, 0);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (Overlay is BiliPlayerOverlay overlay)
        {
            overlay.PaneToggled -= OnOverlayPaneToggled;
        }
    }

    private void OnPanelSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ViewModel?.Player is Player player)
        {
            player?.renderer.ResizeBuffers((int)e.NewSize.Width, (int)e.NewSize.Height);
        }
    }

    private void SwapChainClbk(IDXGISwapChain2 swapChain)
    {
        if (_swapChainPanel == null)
        {
            return;
        }

        using (var nativeObject = SharpGen.Runtime.ComObject.As<Vortice.WinUI.ISwapChainPanelNative2>(_swapChainPanel))
        {
            _ = nativeObject.SetSwapChain(swapChain);
        }

        var player = ViewModel?.Player as Player;
        player?.renderer.ResizeBuffers((int)ActualWidth, (int)ActualHeight);
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Player))
        {
            ReloadPlayer();
        }
    }

    private void ReloadPlayer()
    {
        if (ViewModel?.Player is Player)
        {
            _mediaElement.Visibility = Visibility.Collapsed;
            ReplaceFFmpegPlayer();
        }
        else if (ViewModel?.Player is MediaPlayer mp)
        {
            _swapChainPanel.Visibility = Visibility.Collapsed;
            _mediaElement.SetMediaPlayer(mp);
        }
    }

    private void ReplaceFFmpegPlayer()
    {
        if (ViewModel?.Player == null)
        {
            return;
        }

        var player = (Player)ViewModel.Player;
        player.Host?.Player_Disposed();

        Debug.WriteLine($"链接播放器 {player.PlayerId}");
        player.Host = this;
        Background = new SolidColorBrush(new() { A = player.Config.Video.BackgroundColor.A, R = player.Config.Video.BackgroundColor.R, G = player.Config.Video.BackgroundColor.G, B = player.Config.Video.BackgroundColor.B });
        player.VideoDecoder.CreateSwapChain(SwapChainClbk);
    }
}

/// <summary>
/// <see cref="BiliPlayer"/> 的基类.
/// </summary>
public abstract class BiliPlayerBase : ReactiveControl<IPlayerViewModel>
{
}
