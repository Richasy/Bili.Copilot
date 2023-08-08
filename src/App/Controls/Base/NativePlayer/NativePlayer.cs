// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Media.Playback;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 本机播放器.
/// </summary>
public sealed class NativePlayer : ContentControl
{
    /// <summary>
    /// <see cref="Player"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty PlayerProperty =
        DependencyProperty.Register(nameof(Player), typeof(MediaPlayer), typeof(NativePlayer), new PropertyMetadata(default, new PropertyChangedCallback(OnPlayerChanged)));

    private MediaPlayerElement _mediaElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePlayer"/> class.
    /// </summary>
    public NativePlayer() => DefaultStyleKey = typeof(NativePlayer);

    /// <summary>
    /// 播放器.
    /// </summary>
    public MediaPlayer Player
    {
        get => (MediaPlayer)GetValue(PlayerProperty);
        set => SetValue(PlayerProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        if (GetTemplateChild("MediaElement") is MediaPlayerElement mediaElement)
        {
            _mediaElement = mediaElement;
        }

        if (Player != null)
        {
            LoadPlayer();
        }
    }

    private static void OnPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NativePlayer player && e.NewValue is MediaPlayer)
        {
            player.LoadPlayer();
        }
    }

    private void LoadPlayer()
    {
        if (Player == null)
        {
            return;
        }

        _mediaElement.SetMediaPlayer(Player);
    }
}
