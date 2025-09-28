// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.ViewLater;

/// <summary>
/// 稍后再看主体.
/// </summary>
public sealed partial class ViewLaterBody : ViewLaterPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterBody"/> class.
    /// </summary>
    public ViewLaterBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnVideoListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.ListUpdated -= OnVideoListUpdatedAsync;
    }

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
