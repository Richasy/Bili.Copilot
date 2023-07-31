// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Threading;
using Bili.Copilot.Models.App.Args;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// AI 功能视图模型.
/// </summary>
public sealed partial class AIFeatureViewModel
{
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isTryLaunched;

    /// <summary>
    /// 状态提示列表.
    /// </summary>
    public ObservableCollection<AppTipNotification> Tips { get; set; }
}
