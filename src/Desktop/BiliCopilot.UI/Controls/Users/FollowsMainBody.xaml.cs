// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

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
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.UserListUpdated += OnUserListUpdatedAsync;
        UserScrollView.ViewChanged += OnViewChanged;
        UserScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckUserCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.UserListUpdated -= OnUserListUpdatedAsync;
        UserScrollView.ViewChanged -= OnViewChanged;
        UserScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnUserListUpdatedAsync(object? sender, EventArgs e)
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
                ViewModel.LoadUsersCommand.Execute(default);
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
                ViewModel.LoadUsersCommand.Execute(default);
            }
        });
    }
}
