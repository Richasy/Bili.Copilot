// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 直播弹幕项控件.
/// </summary>
public sealed partial class LiveDanmakuItemControl : LiveDanmakuItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveDanmakuItemControl"/> class.
    /// </summary>
    public LiveDanmakuItemControl() => InitializeComponent();
}

/// <summary>
/// 直播弹幕项控件基类.
/// </summary>
public abstract class LiveDanmakuItemControlBase : LayoutUserControlBase<LiveDanmakuItemViewModel>
{
}
