// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels.DynamicPageViewModel;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 动态页面.
/// </summary>
public sealed partial class DynamicPage : DynamicPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicPage"/> class.
    /// </summary>
    public DynamicPage()
    {
        InitializeComponent();
        ViewModel = DynamicPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        DynamicTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;
        ViewModel.InitializeCommand.Execute(default);
    }

    private async void OnDynamicTypeSegmentedSelectionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        _ = VideoContentScrollViewer.ChangeView(default, 0, default, true);
        _ = AllContentScrollViewer.ChangeView(default, 0, default, true);
        await Task.Delay(100);
        ViewModel.CurrentType = (DynamicDisplayType)DynamicTypeSelection.SelectedIndex;
    }

    private void OnDynamicViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="DynamicPage"/> 的基类.
/// </summary>
public abstract class DynamicPageBase : PageBase<DynamicPageViewModel>
{
}
