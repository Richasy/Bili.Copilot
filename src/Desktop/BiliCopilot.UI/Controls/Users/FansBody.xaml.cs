﻿// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

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
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.UserListUpdated += OnVideoListUpdatedAsync;
        UserScrollView.ViewChanged += OnViewChanged;
        UserScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckVideoCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.UserListUpdated -= OnVideoListUpdatedAsync;
        UserScrollView.ViewChanged -= OnViewChanged;
        UserScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckVideoCount();
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
            CheckVideoCount();
        }
    }

    private void CheckVideoCount()
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
