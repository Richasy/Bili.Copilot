// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.AI;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频播放页头部.
/// </summary>
public sealed partial class VideoPlayerMainHeader : VideoSourceControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerMainHeader"/> class.
    /// </summary>
    public VideoPlayerMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    private void OnAIOpened(object sender, object e)
    {
        var flyout = sender as Flyout;
        if (flyout.Content is null)
        {
            flyout.Content = new AIPanel
            {
                Height = 720,
                ViewModel = ViewModel.AI,
            };
        }

        if (ViewModel.AI.Services is null)
        {
            ViewModel.AI.InitializeCommand.Execute(default);
        }
        else
        {
            if (!string.IsNullOrEmpty(ViewModel.AI.FinalResult))
            {
                var finalResult = ViewModel.AI.FinalResult;
                ViewModel.AI.FinalResult = string.Empty;
                ViewModel.AI.FinalResult = finalResult;
            }

            if (!string.IsNullOrEmpty(ViewModel.AI.RequestText))
            {
                var requestText = ViewModel.AI.RequestText;
                ViewModel.AI.RequestText = string.Empty;
                ViewModel.AI.RequestText = requestText;
            }
        }
    }
}
