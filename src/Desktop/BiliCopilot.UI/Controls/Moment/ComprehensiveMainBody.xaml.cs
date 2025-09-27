// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 综合动态分区主体.
/// </summary>
public sealed partial class ComprehensiveMainBody : MomentUperSectionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveMainBody"/> class.
    /// </summary>
    public ComprehensiveMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(MomentUperSectionViewModel? oldValue, MomentUperSectionViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnListUpdatedAsync;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
