// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Dispatching;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class NextTip : PlayerControlBase
{
    private readonly DispatcherQueueTimer _timer;
    private DateTimeOffset? _visibleTime;

    public NextTip()
    {
        InitializeComponent();
        _timer = DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += OnTimerTick;
    }

    public void Show()
    {
        RootPanel.Visibility = Visibility.Visible;
        _visibleTime = DateTimeOffset.Now;
        _timer.Start();
        ViewModel.IsNextTipShown = true;
    }

    public void Hide()
    {
        ProgressRect.Value = 0;
        RootPanel.Visibility = Visibility.Collapsed;
        _visibleTime = null;
        _timer.Stop();
        ViewModel.IsNextTipShown = false;
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args)
    {
        var now = DateTimeOffset.Now;
        if (_visibleTime.HasValue)
        {
            var ts = Math.Max(0, (now - _visibleTime.Value).TotalSeconds);
            if (ts >= 8)
            {
                RootPanel.Visibility = Visibility.Collapsed;
                ViewModel.PlayNextCommand.Execute(default);
                return;
            }

            ProgressRect.Value = ts;
        }
    }
}
