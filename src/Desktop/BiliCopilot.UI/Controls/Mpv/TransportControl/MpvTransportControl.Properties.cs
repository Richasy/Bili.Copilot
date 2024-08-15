// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Mpv;

/// <summary>
/// 播放器传输控件.
/// </summary>
public sealed partial class MpvTransportControl
{
    /// <summary>
    /// <see cref="LeftContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty LeftContentProperty =
        DependencyProperty.Register(nameof(LeftContent), typeof(object), typeof(MpvTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="MiddleContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty MiddleContentProperty =
        DependencyProperty.Register(nameof(MiddleContent), typeof(object), typeof(MpvTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="RightContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty RightContentProperty =
        DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(MpvTransportControl), new PropertyMetadata(default));

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
}
