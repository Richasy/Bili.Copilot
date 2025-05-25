// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频操作区域.
/// </summary>
public sealed partial class VideoOperationControl : VideoPlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoOperationControl"/> class.
    /// </summary>
    public VideoOperationControl() => InitializeComponent();

    private void OnLoopSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LoopComboBox.SelectedItem is null)
        {
            return;
        }

        ViewModel.CurrentLoop = (VideoLoopType)LoopComboBox.SelectedItem;
    }
}
