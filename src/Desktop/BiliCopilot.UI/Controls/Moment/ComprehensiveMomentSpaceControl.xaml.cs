// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 综合动态空间控件.
/// </summary>
public sealed partial class ComprehensiveMomentSpaceControl : UserMomentDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveMomentSpaceControl"/> class.
    /// </summary>
    public ComprehensiveMomentSpaceControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnListUpdatedAsync;
        }
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
