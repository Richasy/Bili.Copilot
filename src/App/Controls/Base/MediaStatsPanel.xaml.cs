// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.App.Other;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 媒体信息面板.
/// </summary>
public sealed partial class MediaStatsPanel : MediaStatsPanelBase
{
    /// <summary>
    /// <see cref="AdditionalContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AdditionalContentProperty =
       DependencyProperty.Register(nameof(AdditionalContent), typeof(object), typeof(MediaStatsPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaStatsPanel"/> class.
    /// </summary>
    public MediaStatsPanel() => InitializeComponent();

    /// <summary>
    /// 附加内容.
    /// </summary>
    public object AdditionalContent
    {
        get => GetValue(AdditionalContentProperty);
        set => SetValue(AdditionalContentProperty, value);
    }
}

/// <summary>
/// <see cref="MediaStatsPanel"/> 的基类.
/// </summary>
public abstract class MediaStatsPanelBase : ReactiveUserControl<MediaStats>
{
}
