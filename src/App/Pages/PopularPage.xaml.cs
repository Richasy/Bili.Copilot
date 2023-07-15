// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
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
        PopularTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;
        ViewModel.InitializeCommand.Execute(default);
    }

    private async void OnPopularTypeSegmentedSelectionChangedAsync(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        ContentScrollViewer.ChangeView(default, 0, default, true);
        await Task.Delay(100);
        ViewModel.CurrentType = (PopularType)PopularTypeSelection.SelectedIndex;
    }

    private void OnVideoViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);
}

/// <summary>
/// <see cref="PopularPage"/> 的基类.
/// </summary>
public abstract class PopularPageBase : PageBase<PopularPageViewModel>
{
}
