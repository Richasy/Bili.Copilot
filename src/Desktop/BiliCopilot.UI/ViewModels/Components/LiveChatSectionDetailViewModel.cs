﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;
using System.Collections.ObjectModel;
using System.Net.WebSockets;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 直播聊天区域详情视图模型.
/// </summary>
public sealed partial class LiveChatSectionDetailViewModel : ViewModelBase
{
    private readonly IPlayerService _service;
    private readonly ILogger<LiveChatSectionDetailViewModel> _logger;
    private DispatcherTimer _heartBeatTimer;

    private Action<string> _displayDanmakuAction;
    private Func<string, Task> _sendDanmakuFunc;
    private string _roomId;
    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isSending;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveChatSectionDetailViewModel"/> class.
    /// </summary>
    public LiveChatSectionDetailViewModel(
        IPlayerService service,
        LivePlayerPageViewModel pageVM)
    {
        _service = service;
        Page = pageVM;
        _logger = this.Get<ILogger<LiveChatSectionDetailViewModel>>();
        IsEmpty = true;
    }

    /// <summary>
    /// 滚动到底部.
    /// </summary>
    public event EventHandler ScrollToBottomRequested;

    /// <summary>
    /// 消息集合.
    /// </summary>
    public ObservableCollection<LiveDanmakuItemViewModel> Messages { get; } = new();

    /// <summary>
    /// 页面.
    /// </summary>
    public LivePlayerPageViewModel Page { get; }

    /// <summary>
    /// 开始监听.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task StartAsync(string roomId, Action<string> displayAction, Func<string, Task> sendDanmakuFunc)
    {
        _roomId = roomId;
        _displayDanmakuAction = displayAction;
        _sendDanmakuFunc = sendDanmakuFunc;
        if (_webSocket is not null || _cancellationTokenSource != null)
        {
            await CloseAsync();
        }

        if (_heartBeatTimer is null)
        {
            _heartBeatTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(25),
            };
            _heartBeatTimer.Tick += OnHeartBeatTimerTickAsync;
        }

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _webSocket = await _service.EnterLiveRoomAsync(roomId, _cancellationTokenSource.Token);
            MessageLoop();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "进入直播间失败");
        }
    }

    /// <summary>
    /// 关闭.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task CloseAsync()
    {
        _heartBeatTimer.Stop();
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, Messages.Clear);
        await _cancellationTokenSource?.CancelAsync();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = default;
        if (_webSocket?.State == WebSocketState.Open)
        {
            await _webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        _webSocket?.Dispose();
        _webSocket = null;
    }

    [RelayCommand]
    private async Task SendDanmakuAsync(string text)
    {
        IsSending = true;
        await _sendDanmakuFunc(text);
        IsSending = false;
    }

    [RelayCommand]
    private void ScrollToBottom()
        => ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);

    private void MessageLoop()
    {
        _ = Task.Run(async () =>
        {
            while (_webSocket is not null && !_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var messages = await _service.GetLiveSocketMessagesAsync(_webSocket, _cancellationTokenSource.Token);
                    if (messages is not null)
                    {
                        foreach (var msg in messages)
                        {
                            if (msg.Type == Richasy.BiliKernel.Models.LiveMessageType.ConnectSuccess)
                            {
                                this.Get<Microsoft.UI.Dispatching.DispatcherQueue>()
                                    .TryEnqueue(() =>
                                    {
                                        _heartBeatTimer.Start();
                                    });
                            }
                            else if (msg.Type == Richasy.BiliKernel.Models.LiveMessageType.Danmaku && msg.Data is LiveDanmakuInformation danmaku)
                            {
                                this.Get<Microsoft.UI.Dispatching.DispatcherQueue>()
                                    .TryEnqueue(() =>
                                    {
                                        if (_cancellationTokenSource is not null && _webSocket is not null && !_cancellationTokenSource.IsCancellationRequested)
                                        {
                                            if (Messages.Count > 2000)
                                            {
                                                // 移除前1000条消息.
                                                for (var i = 0; i < 1000; i++)
                                                {
                                                    Messages.RemoveAt(0);
                                                }
                                            }

                                            Messages.Add(new LiveDanmakuItemViewModel(danmaku));
                                            IsEmpty = false;
                                            _displayDanmakuAction?.Invoke(danmaku.Text);
                                            ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
                                        }
                                    });
                            }
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解析直播消息时出现异常");
                }
            }
        });
    }

    private async void OnHeartBeatTimerTickAsync(object? sender, object e)
    {
        try
        {
            await _service.SendLiveHeartBeatAsync(_webSocket, _cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "发送心跳包时出现异常");
        }
    }
}
