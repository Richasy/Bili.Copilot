// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 直播分区页面.
/// </summary>
public sealed partial class LivePartitionPage : LivePartitionPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionPage"/> class.
    /// </summary>
    public LivePartitionPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 直播分区页面基类.
/// </summary>
public abstract class LivePartitionPageBase : LayoutPageBase<LivePartitionPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionPageBase"/> class.
    /// </summary>
    protected LivePartitionPageBase() => ViewModel = this.Get<LivePartitionPageViewModel>();
}
