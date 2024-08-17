// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// 视频播放器控件.
/// </summary>
public sealed partial class VideoTransportControl : PlayerControlBase
{
    /// <summary>
    /// <see cref="LeftContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty LeftContentProperty =
        DependencyProperty.Register(nameof(LeftContent), typeof(object), typeof(VideoTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="MiddleContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty MiddleContentProperty =
        DependencyProperty.Register(nameof(MiddleContent), typeof(object), typeof(VideoTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="RightContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty RightContentProperty =
        DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(VideoTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoTransportControl"/> class.
    /// </summary>
    public VideoTransportControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 左侧内容.
    /// </summary>
    public object LeftContent
    {
        get => (object)GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    /// <summary>
    /// 中部内容.
    /// </summary>
    public object MiddleContent
    {
        get => (object)GetValue(MiddleContentProperty);
        set => SetValue(MiddleContentProperty, value);
    }

    /// <summary>
    /// 右侧内容.
    /// </summary>
    public object RightContent
    {
        get => (object)GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    private void OnVolumeSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel?.SetVolumeCommand.Execute(Convert.ToInt32(e.NewValue));

    private void OnProgressSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel?.SeekCommand.Execute(Convert.ToInt32(e.NewValue));

    private void OnSpeedSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel?.SetSpeedCommand.Execute(Math.Round(e.NewValue, 1));
}
