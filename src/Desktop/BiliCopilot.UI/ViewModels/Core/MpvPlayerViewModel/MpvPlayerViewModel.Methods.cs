// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class MpvPlayerViewModel
{
    private static string GetScreenshotFolderPath()
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bili-Screenshots");

    private void InitializeDecode()
    {
        var decodeType = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Software);
        switch (decodeType)
        {
            case PreferDecodeType.Software:
                Player.Client.SetProperty("hwdec", "no");
                Player.Client.SetProperty("gpu-context", "auto");
                Player.Client.SetProperty("gpu-api", "auto");
                break;
            case PreferDecodeType.D3D11:
                Player.Client.SetProperty("hwdec", "d3d11va");
                Player.Client.SetProperty("gpu-context", "d3d11");
                Player.Client.SetProperty("gpu-api", "d3d11");
                break;
            case PreferDecodeType.NVDEC:
                Player.Client.SetProperty("hwdec", "nvdec");
                Player.Client.SetProperty("gpu-context", "auto");
                Player.Client.SetProperty("gpu-api", "auto");
                break;
            case PreferDecodeType.DXVA2:
                Player.Client.SetProperty("hwdec", "dxva2");
                Player.Client.SetProperty("gpu-context", "dxinterop");
                Player.Client.SetProperty("gpu-api", "auto");
                break;
            default:
                break;
        }
    }

    private async Task TryLoadPlayDataAsync()
    {
        if (string.IsNullOrEmpty(_videoUrl) && string.IsNullOrEmpty(_audioUrl))
        {
            return;
        }

        InitializeDecode();
        if (!_autoPlay)
        {
            Player.Client.SetOption("pause", "yes");
        }
        else
        {
            Player.Client.SetOption("pause", "no");
        }

        Player.Client.SetOption("start", Position.ToString());

        IsPlayerDataLoading = true;
        IsPaused = true;

        Player.SetVolume(Volume);
        Player.SetSpeed(Speed);

        if (!string.IsNullOrEmpty(_videoUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _videoUrl, "replace"]);

            if (!string.IsNullOrEmpty(_audioUrl))
            {
                await WaitUntilAddAudioAsync(_audioUrl);
            }
        }
        else if (!string.IsNullOrEmpty(_audioUrl))
        {
            await Player.Client.ExecuteAsync(["loadfile", _audioUrl, "replace"]);
        }

        if (!IsLive)
        {
            Player.ResetDuration();
            Duration = Convert.ToInt32(Player.Duration!.Value.TotalSeconds);
        }

        PlayerDataLoaded?.Invoke(this, EventArgs.Empty);
        IsPlayerDataLoading = false;
    }

    private async Task WaitUntilAddAudioAsync(string audioUrl)
    {
        const int maxRetryCount = 10;
        var retryCount = 0;
        var isAudioAdded = false;
        do
        {
            if (retryCount >= maxRetryCount)
            {
                break;
            }

            if (!Player.IsMediaLoaded())
            {
                await Task.Delay(300);
                continue;
            }

            try
            {
                await Player.Client.ExecuteAsync(["audio-add", audioUrl]);
                isAudioAdded = true;
            }
            catch (Exception)
            {
                retryCount++;
                await Task.Delay(300);
            }
        }
        while (!isAudioAdded);
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
