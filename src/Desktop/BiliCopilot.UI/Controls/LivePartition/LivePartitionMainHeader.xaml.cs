// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播分区主页头部.
/// </summary>
public sealed partial class LivePartitionMainHeader : LivePartitionPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionMainHeader"/> class.
    /// </summary>
    public LivePartitionMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);
}
