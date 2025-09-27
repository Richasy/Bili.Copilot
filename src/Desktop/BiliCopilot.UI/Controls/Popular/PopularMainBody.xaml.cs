// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Popular;

/// <summary>
/// 流行视频页面主体.
/// </summary>
public sealed partial class PopularMainBody : PopularPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularMainBody"/> class.
    /// </summary>
    public PopularMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.VideoListUpdated += OnVideoListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.VideoListUpdated -= OnVideoListUpdatedAsync;
    }

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
