// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Users;

/// <summary>
/// 关注页面主体.
/// </summary>
public sealed partial class FollowsMainBody : FollowsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FollowsMainBody"/> class.
    /// </summary>
    public FollowsMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.UserListUpdated += OnUserListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.UserListUpdated -= OnUserListUpdatedAsync;
    }

    private async void OnUserListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
