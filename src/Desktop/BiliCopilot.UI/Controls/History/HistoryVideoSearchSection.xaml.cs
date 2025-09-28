// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 历史记录视频搜索部分.
/// </summary>
public sealed partial class HistoryVideoSearchSection : HistoryPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryVideoSearchSection"/> class.
    /// </summary>
    public HistoryVideoSearchSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.SearchUpdated += OnSearchUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SearchUpdated -= OnSearchUpdatedAsync;
    }

    private async void OnSearchUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
