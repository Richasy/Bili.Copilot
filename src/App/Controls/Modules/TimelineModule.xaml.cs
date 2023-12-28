// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 时间线模块.
/// </summary>
public sealed partial class TimelineModule : TimelineModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineModule"/> class.
    /// </summary>
    public TimelineModule()
    {
        InitializeComponent();
        ViewModel = TimelineViewModel.Instance;
    }
}

/// <summary>
/// <see cref="TimelineModule"/> 的基类.
/// </summary>
public abstract class TimelineModuleBase : ReactiveUserControl<TimelineViewModel>
{
}
