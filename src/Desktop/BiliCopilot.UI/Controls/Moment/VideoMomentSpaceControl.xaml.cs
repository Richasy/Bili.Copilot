// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 视频动态空间控件.
/// </summary>
public sealed partial class VideoMomentSpaceControl : UserMomentDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoMomentSpaceControl"/> class.
    /// </summary>
    public VideoMomentSpaceControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.ListUpdated -= OnListUpdatedAsync;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
