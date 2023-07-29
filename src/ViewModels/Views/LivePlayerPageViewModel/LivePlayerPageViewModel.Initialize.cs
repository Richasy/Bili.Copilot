// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播播放页面视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    private void InitializeTimers()
    {
        if (_heartBeatTimer == null)
        {
            _heartBeatTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(25),
            };
            _heartBeatTimer.Tick += OnHeartBeatTimerTickAsync;
        }
    }

    private void InitializePublisher()
    {
        var profile = View.Information.User;
        var vm = new UserItemViewModel(profile);
        User = vm;
        User.InitializeRelationCommand.Execute(default);
    }

    private void InitializeOverview()
        => WatchingCountText = NumberToolkit.GetCountText(View.Information.ViewerCount);
}
