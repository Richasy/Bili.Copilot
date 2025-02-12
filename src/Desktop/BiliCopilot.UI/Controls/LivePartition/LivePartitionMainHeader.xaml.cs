// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Controls.Primitives;
using Richasy.WinUIKernel.Share.Base;

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

    private void OnFollowRoomButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
