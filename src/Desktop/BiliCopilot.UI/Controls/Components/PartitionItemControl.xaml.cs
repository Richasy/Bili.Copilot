// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 分区条目控件.
/// </summary>
public sealed partial class PartitionItemControl : PartitionItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionItemControl"/> class.
    /// </summary>
    public PartitionItemControl() => InitializeComponent();
}

/// <summary>
/// 分区控件基类.
/// </summary>
public abstract class PartitionItemControlBase : LayoutUserControlBase<PartitionViewModel>
{
}
