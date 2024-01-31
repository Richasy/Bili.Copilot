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
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.RequestScrollToTop += OnRequestScrollToTopAsync;

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.RequestScrollToTop -= OnRequestScrollToTopAsync;

    private async void OnRequestScrollToTopAsync(object sender, EventArgs e)
    {
        await Task.Delay(500);
        ContentScrollViewer.ChangeView(default, 0, default);
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
