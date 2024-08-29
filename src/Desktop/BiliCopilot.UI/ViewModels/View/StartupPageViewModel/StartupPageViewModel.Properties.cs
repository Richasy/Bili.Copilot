// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Authorization;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 启动页视图模型.
/// </summary>
public sealed partial class StartupPageViewModel
{
    private readonly ILogger<StartupPageViewModel> _logger;
    private readonly IAuthenticationService _authenticationService;
    private readonly DispatcherQueue _dispatcherQueue;

    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private string _version;

    [ObservableProperty]
    private string _errorTip;

    [ObservableProperty]
    private bool _isQRCodeLoading;

    /// <summary>
    /// 二维码图片控件.
    /// </summary>
    public Image? QRCodeImage { get; private set; }
}
