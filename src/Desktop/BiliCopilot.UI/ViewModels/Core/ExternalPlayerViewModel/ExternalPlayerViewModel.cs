// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 外部播放器视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class ExternalPlayerViewModel : PlayerViewModelBase
{
    private readonly List<string> _mpvDebugMessages = new();

    [ObservableProperty]
    private string _logMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalPlayerViewModel"/> class.
    /// </summary>
    public ExternalPlayerViewModel() => _isInitialized = true;

    /// <summary>
    /// 外部播放器进程.
    /// </summary>
    public Process? PlayerProcess { get; private set; }

    private void ResetPlayer()
    {
        _mpvDebugMessages?.Clear();
        _dispatcherQueue?.TryEnqueue(() =>
        {
            LogMessage = string.Empty;
        });

        if (PlayerProcess is not null)
        {
            PlayerProcess.ErrorDataReceived -= OnPlayerProcessDataReceived;
            PlayerProcess.OutputDataReceived -= OnPlayerProcessDataReceived;
            PlayerProcess.Kill();
            PlayerProcess.Dispose();
            PlayerProcess = null;
        }
    }
}
