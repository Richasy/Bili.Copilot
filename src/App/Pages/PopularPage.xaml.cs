// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 流行视频页面.
/// </summary>
public sealed partial class PopularPage : PopularPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPage"/> class.
    /// </summary>
    public PopularPage()
    {
        InitializeComponent();
        ViewModel = PopularPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ViewModel.RequestScrollToTop += OnRequestScrollToTopAsync;
        ViewModel.InitializeCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.RequestScrollToTop -= OnRequestScrollToTopAsync;

    private void OnVideoViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private async void OnRequestScrollToTopAsync(object sender, EventArgs e)
    {
        await Task.Delay(500);
        ContentScrollViewer?.ChangeView(0, 0, default);
    }
}

/// <summary>
/// <see cref="PopularPage"/> 的基类.
/// </summary>
public abstract class PopularPageBase : PageBase<PopularPageViewModel>
{
}
