// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 播放器通知.
/// </summary>
public sealed partial class PlayerNotificationItemViewModel : ViewModelBase<PlayerNotification>
{
    private readonly DispatcherTimer? _timer;

    private DateTimeOffset _startTime;
    private bool _isCancelled;

    [ObservableProperty]
    private int _durationInMilliseconds;

    [ObservableProperty]
    private int _progressInMilliseconds;

    [ObservableProperty]
    private bool _isProgressVisible;

    [ObservableProperty]
    private bool _isNotificationVisible;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotificationItemViewModel"/> class.
    /// </summary>
    public PlayerNotificationItemViewModel(PlayerNotification notification)
        : base(notification)
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += OnTimerTick;
    }

    [RelayCommand]
    private void Start()
    {
        _startTime = DateTimeOffset.Now;
        DurationInMilliseconds = Data.Duration * 1000;
        ProgressInMilliseconds = 0;
        IsProgressVisible = true;
        IsNotificationVisible = true;
        _timer.Start();
    }

    [RelayCommand]
    private void Cancel()
    {
        _isCancelled = true;
        IsProgressVisible = false;
        IsNotificationVisible = false;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }

    [RelayCommand]
    private void Active()
    {
        _timer.Stop();
        _timer.Tick -= OnTimerTick;

        if (!_isCancelled)
        {
            Data.Action?.Invoke();
        }

        IsNotificationVisible = false;
    }

    private void OnTimerTick(object? sender, object e)
    {
        var p = (int)(DateTimeOffset.Now - _startTime).TotalMilliseconds;
        if (p >= DurationInMilliseconds)
        {
            IsProgressVisible = false;
            Active();
        }
        else
        {
            ProgressInMilliseconds = p;
        }
    }
}
