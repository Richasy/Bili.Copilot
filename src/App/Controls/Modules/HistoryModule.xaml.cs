// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 历史记录模块.
/// </summary>
public sealed partial class HistoryModule : HistoryModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryModule"/> class.
    /// </summary>
    public HistoryModule()
    {
        InitializeComponent();
        ViewModel = HistoryDetailViewModel.Instance;
    }

    private void OnVideoViewIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="HistoryModule"/> 的基类.
/// </summary>
public abstract class HistoryModuleBase : ReactiveUserControl<HistoryDetailViewModel>
{
}
