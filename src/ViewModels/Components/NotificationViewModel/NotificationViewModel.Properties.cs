// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Timers;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel
{
    private static readonly Lazy<NotificationViewModel> _lazyInstance = new(() => new NotificationViewModel());
    private readonly Timer _timer;
    private bool _isTileSupport;

    /// <summary>
    /// 实例.
    /// </summary>
    public static NotificationViewModel Instance => _lazyInstance.Value;
}
