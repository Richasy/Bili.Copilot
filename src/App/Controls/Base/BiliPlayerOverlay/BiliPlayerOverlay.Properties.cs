// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public partial class BiliPlayerOverlay
{
    /// <summary>
    /// <see cref="MediaPresenter"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty MediaPresenterProperty =
        DependencyProperty.Register(nameof(MediaPresenter), typeof(object), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IsLive"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsLiveProperty =
        DependencyProperty.Register(nameof(IsLive), typeof(bool), typeof(BiliPlayerOverlay), new PropertyMetadata(default));

    /// <summary>
    /// 光标是否停留在覆盖层上.
    /// </summary>
    public bool IsPointerStay { get; set; }

    /// <summary>
    /// 播放信息展示.
    /// </summary>
    public object MediaPresenter
    {
        get => (object)GetValue(MediaPresenterProperty);
        set => SetValue(MediaPresenterProperty, value);
    }

    /// <summary>
    /// 是否为直播.
    /// </summary>
    public bool IsLive
    {
        get => (bool)GetValue(IsLiveProperty);
        set => SetValue(IsLiveProperty, value);
    }
}
