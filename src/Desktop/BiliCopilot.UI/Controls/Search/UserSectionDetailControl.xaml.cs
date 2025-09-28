// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 用户搜索分区详情控件.
/// </summary>
public sealed partial class UserSectionDetailControl : UserSectionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSectionDetailControl"/> class.
    /// </summary>
    public UserSectionDetailControl() => InitializeComponent();

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

/// <summary>
/// 用户搜索分区详情控件基类.
/// </summary>
public abstract class UserSectionDetailControlBase : LayoutUserControlBase<UserSearchSectionDetailViewModel>
{
}
