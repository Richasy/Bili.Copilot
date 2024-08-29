// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel
{
    private readonly ILogger<NotificationViewModel> _logger;
    private readonly IMomentDiscoveryService _momentService;

    private DispatcherTimer _timer;
    private bool _isTileSupport;

    private List<string> _momentIds;
}
