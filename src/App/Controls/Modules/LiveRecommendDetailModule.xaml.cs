// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 直播推荐模块.
/// </summary>
public sealed partial class LiveRecommendDetailModule : LiveRecommendDetailModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveRecommendDetailModule"/> class.
    /// </summary>
    public LiveRecommendDetailModule()
    {
        InitializeComponent();
        ViewModel = LiveRecommendDetailViewModel.Instance;
    }

    private void OnLiveViewIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="LiveRecommendDetailModule"/> 的基类.
/// </summary>
public abstract class LiveRecommendDetailModuleBase : ReactiveUserControl<LiveRecommendDetailViewModel>
{
}
