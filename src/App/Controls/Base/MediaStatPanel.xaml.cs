// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.App.Other;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 媒体信息面板.
/// </summary>
public sealed partial class MediaStatPanel : MediaStatPanelBase
{
    /// <summary>
    /// <see cref="AdditionalContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AdditionalContentProperty =
       DependencyProperty.Register(nameof(AdditionalContent), typeof(object), typeof(MediaStatPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaStatPanel"/> class.
    /// </summary>
    public MediaStatPanel()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 附加内容.
    /// </summary>
    public object AdditionalContent
    {
        get => (object)GetValue(AdditionalContentProperty);
        set => SetValue(AdditionalContentProperty, value);
    }
}

/// <summary>
/// <see cref="MediaStatPanel"/> 的基类.
/// </summary>
public abstract class MediaStatPanelBase : ReactiveUserControl<MediaStats>
{
}
