// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.VideoPartition;

/// <summary>
/// 视频分区详情控件.
/// </summary>
public sealed partial class VideoPartitionItemControl : VideoPartitionItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionItemControl"/> class.
    /// </summary>
    public VideoPartitionItemControl()
    {
        InitializeComponent();
    }
}

/// <summary>
/// 视频分区详情控件基类.
/// </summary>
public abstract class VideoPartitionItemControlBase : LayoutUserControlBase<VideoPartitionViewModel>
{
}
