// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// WebDAV 页面.
/// </summary>
public sealed partial class WebDavPage : WebDavPageBase
{
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPage"/> class.
    /// </summary>
    public WebDavPage()
    {
        InitializeComponent();
        ViewModel = new WebDavPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        LayoutPicker.SelectedIndex = ViewModel.IsListLayout ? 0 : 1;
        ViewModel.InitializeCommand.Execute(default);
        _isInitialized = true;
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as WebDavStorageItemViewModel;
        if (data.IsFolder)
        {
            ViewModel.LoadPathCommand.Execute(data.Data.Href);
        }
        else
        {
            ViewModel.OpenVideoCommand.Execute(data);
        }
    }

    private void OnPathSegmentClick(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var seg = args.Item as WebDavPathSegment;
        ViewModel.LoadPathCommand.Execute(seg.Path);
    }

    private void OnLayoutPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isInitialized)
        {
            return;
        }

        ViewModel.IsListLayout = LayoutPicker.SelectedIndex == 0;
    }
}

/// <summary>
/// WebDAV 页面基类.
/// </summary>
public abstract class WebDavPageBase : PageBase<WebDavPageViewModel>
{
}
