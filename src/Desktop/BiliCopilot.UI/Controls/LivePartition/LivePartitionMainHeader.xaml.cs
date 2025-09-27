// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Controls.Primitives;

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

    private void OnFollowRoomButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
