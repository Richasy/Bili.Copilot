// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 外部播放器控件.
/// </summary>
public sealed partial class ExternalTransportControl : PlayerControlBase
{
    /// <summary>
    /// <see cref="LeftContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty LeftContentProperty =
        DependencyProperty.Register(nameof(LeftContent), typeof(object), typeof(VideoTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="RightContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty RightContentProperty =
        DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(VideoTransportControl), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalTransportControl"/> class.
    /// </summary>
    public ExternalTransportControl() => InitializeComponent();

    /// <summary>
    /// 左侧内容.
    /// </summary>
    public object LeftContent
    {
        get => (object)GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
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
