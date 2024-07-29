﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Popular;

/// <summary>
/// 流行视频区块项控件.
/// </summary>
public sealed partial class PopularSectionItemControl : PopularSectionItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularSectionItemControl"/> class.
    /// </summary>
    public PopularSectionItemControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);
}

/// <summary>
/// 流行视频区块项控件基类.
/// </summary>
public abstract class PopularSectionItemControlBase : LayoutUserControlBase<PopularSectionItemViewModel>
{
}
