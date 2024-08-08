// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 我的动态主体.
/// </summary>
public sealed partial class MyMomentsMainBody : MyMomentsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyMomentsMainBody"/> class.
    /// </summary>
    public MyMomentsMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        MomentScrollView.ViewChanged += OnViewChanged;
        MomentScrollView.SizeChanged += OnScrollViewSizeChanged;

        ViewModel.ListUpdated += OnListUpdatedAsync;
        CheckMomentCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnListUpdatedAsync;
        }

        MomentScrollView.ViewChanged -= OnViewChanged;
        MomentScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckMomentCount();
    }

    private void OnViewChanged(ScrollView sender, object args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (MomentScrollView.ExtentHeight - MomentScrollView.ViewportHeight - MomentScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckMomentCount();
        }
    }

    private void CheckMomentCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (MomentScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}
