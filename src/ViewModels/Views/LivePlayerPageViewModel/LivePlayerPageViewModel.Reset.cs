// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Provider;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播播放页面视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    private void ResetTimers()
        => _heartBeatTimer?.Stop();

    private void ResetPublisher()
        => User = null;

    private void ResetOverview()
    {
        IsError = false;
        WatchingCountText = default;
    }

    private void ResetInterop()
    {
        IsLiveFixed = false;
        IsDanmakusAutoScroll = true;
        TryClear(Danmakus);
        LiveProvider.Instance.MessageReceived -= OnMessageReceived;
        LiveProvider.Instance.ResetLiveConnection();
    }
}
