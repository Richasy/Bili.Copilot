﻿// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    private readonly ILogger<AppViewModel> _logger;

    [ObservableProperty]
    private Window _activatedWindow;

    /// <summary>
    /// 已创建的窗口列表.
    /// </summary>
    public List<Window> Windows { get; } = new();
}
