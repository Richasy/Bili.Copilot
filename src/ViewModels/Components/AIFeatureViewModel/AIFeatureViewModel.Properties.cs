// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Threading;
using Bili.Copilot.Models.App.Args;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel.AppService;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// AI 功能视图模型.
/// </summary>
public sealed partial class AIFeatureViewModel
{
    private CancellationTokenSource _cancellationTokenSource;
    private AppServiceConnection _connection;
    private bool _isTryLaunched;

    [ObservableProperty]
    private bool _isWaiting;

    [ObservableProperty]
    private string _responseText;

    /// <summary>
    /// 状态提示列表.
    /// </summary>
    public ObservableCollection<AppTipNotification> Tips { get; set; }
}
