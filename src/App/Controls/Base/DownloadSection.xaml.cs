// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 下载区块.
/// </summary>
public sealed partial class DownloadSection : DownloadSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadSection"/> class.
    /// </summary>
    public DownloadSection() => InitializeComponent();

    private void OnPartItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement)?.DataContext as VideoIdentifierSelectableViewModel;
        context.IsSelected = !context.IsSelected;
        ViewModel.CheckSelectAllStatusCommand.Execute(default);
    }

    private void OnDownloadButtonClick(object sender, RoutedEventArgs e)
        => TraceLogger.LogDownload(ViewModel.VideoType.ToString());
}

/// <summary>
/// <see cref="DownloadSection"/> 的基类.
/// </summary>
public abstract class DownloadSectionBase : ReactiveUserControl<DownloadModuleViewModel>
{
}
