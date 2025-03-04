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
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnListUpdatedAsync;
        UserScrollView.ViewChanged += OnViewChanged;
        UserScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckUserCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.ListUpdated -= OnListUpdatedAsync;
        UserScrollView.ViewChanged -= OnViewChanged;
        UserScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckUserCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (UserScrollView.ExtentHeight - UserScrollView.ViewportHeight - UserScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckUserCount();
        }
    }

    private void CheckUserCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (UserScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 用户搜索分区详情控件基类.
/// </summary>
public abstract class UserSectionDetailControlBase : LayoutUserControlBase<UserSearchSectionDetailViewModel>
{
}
