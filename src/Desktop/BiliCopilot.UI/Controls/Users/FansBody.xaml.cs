// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Users;

/// <summary>
/// 粉丝页面主体.
/// </summary>
public sealed partial class FansBody : FansPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansBody"/> class.
    /// </summary>
    public FansBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.UserListUpdated += OnVideoListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.UserListUpdated -= OnVideoListUpdatedAsync;
    }

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
