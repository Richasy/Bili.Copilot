// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 时间轴控件.
/// </summary>
public sealed partial class TimelineControl : TimelineControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineControl"/> class.
    /// </summary>
    public TimelineControl() => InitializeComponent();
}

/// <summary>
/// 时间轴控件基类.
/// </summary>
public abstract class TimelineControlBase : LayoutUserControlBase<TimelineItemViewModel>
{
}
