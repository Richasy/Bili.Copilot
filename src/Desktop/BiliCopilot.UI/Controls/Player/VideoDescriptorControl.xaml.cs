// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频描述组件.
/// </summary>
public sealed partial class VideoDescriptorControl : VideoPlayerControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoDescriptorControl"/> class.
    /// </summary>
    public VideoDescriptorControl() => InitializeComponent();

    private void OnTagButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as BiliTag;
        this.Get<SearchBoxViewModel>().SearchCommand.Execute(data.Name);
    }
}
