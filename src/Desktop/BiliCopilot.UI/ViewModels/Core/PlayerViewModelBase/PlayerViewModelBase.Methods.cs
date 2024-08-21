// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型基类.
/// </summary>
public abstract partial class PlayerViewModelBase
{
    private static string GetScreenshotFolderPath()
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bili-Screenshots");

    private async Task TryLoadPlayDataAsync()
    {
        if (string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl))
        {
            return;
        }

        IsPlayerDataLoading = true;
        IsPaused = true;

        SetVolume(Volume);
        SetSpeed(Speed);

        await OnLoadPlayDataAsync();

        PlayerDataLoaded?.Invoke(this, EventArgs.Empty);
        IsPlayerDataLoading = false;
    }

    private void ActiveDisplay()
    {
        if (_displayRequest != null)
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest = new Windows.System.Display.DisplayRequest();
            _displayRequest.RequestActive();
        });
    }

    private void ReleaseDisplay()
    {
        if (_displayRequest == null)
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest?.RequestRelease();
            _displayRequest = null;
        });
    }
}
