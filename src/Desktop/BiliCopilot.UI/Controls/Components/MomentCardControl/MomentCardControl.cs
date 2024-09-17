// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 动态卡片控件.
/// </summary>
public sealed partial class MomentCardControl : LayoutControlBase<MomentItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentCardControl"/> class.
    /// </summary>
    public MomentCardControl() => DefaultStyleKey = typeof(MomentCardControl);
}
