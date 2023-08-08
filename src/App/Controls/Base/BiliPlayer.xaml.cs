// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器.
/// </summary>
public sealed partial class BiliPlayer : BiliPlayerBase
{
    /// <summary>
    /// <see cref="IsFFmpeg"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsFFmpegProperty =
        DependencyProperty.Register(nameof(IsFFmpeg), typeof(bool), typeof(BiliPlayer), new PropertyMetadata(true));

    /// <summary>
    /// <see cref="Overlay"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty OverlayProperty =
        DependencyProperty.Register(nameof(Overlay), typeof(object), typeof(BiliPlayer), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayer"/> class.
    /// </summary>
    public BiliPlayer() => InitializeComponent();

    /// <summary>
    /// 是否为 FFmpeg 播放器.
    /// </summary>
    public bool IsFFmpeg
    {
        get => (bool)GetValue(IsFFmpegProperty);
        set => SetValue(IsFFmpegProperty, value);
    }

    /// <summary>
    /// 覆盖层.
    /// </summary>
    public object Overlay
    {
        get => (object)GetValue(OverlayProperty);
        set => SetValue(OverlayProperty, value);
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is PlayerDetailViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is PlayerDetailViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChanged;
        }

        ReloadPlayer();
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
        if (ViewModel.Player is FFmpegPlayerViewModel)
        {
            IsFFmpeg = true;
        }
    }
}

/// <summary>
/// <see cref="BiliPlayer"/> 的基类.
/// </summary>
public abstract class BiliPlayerBase : ReactiveUserControl<PlayerDetailViewModel>
{
}

internal sealed class FFmpegPlayerConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is Player player ? player : (object)default;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
