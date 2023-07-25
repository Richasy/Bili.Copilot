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
    /// 播放信息展示.
    /// </summary>
    public object MediaPresenter
    {
        get => (object)GetValue(MediaPresenterProperty);
        set => SetValue(MediaPresenterProperty, value);
    }
}
