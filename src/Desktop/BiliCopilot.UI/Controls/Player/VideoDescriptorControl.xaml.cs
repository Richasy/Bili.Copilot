// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Base;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频描述组件.
/// </summary>
public sealed partial class VideoDescriptorControl : VideoPlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoDescriptorControl"/> class.
    /// </summary>
    public VideoDescriptorControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    private void OnTagButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as BiliTag;
        this.Get<SearchBoxViewModel>().SearchCommand.Execute(data.Name);
    }
}
