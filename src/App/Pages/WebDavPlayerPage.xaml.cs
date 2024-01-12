// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;
using Microsoft.UI.Windowing;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// WebDAV 播放器页面.
/// </summary>
public sealed partial class WebDavPlayerPage : WebDavPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPage"/> class.
    /// </summary>
    public WebDavPlayerPage()
    {
        InitializeComponent();
        ViewModel = new WebDavPlayerPageViewModel();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is List<WebDavStorageItemViewModel> items)
        {
            ViewModel.InitializeCommand.Execute(items);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        PlayerElement.SetMediaPlayer(default);
        ViewModel.ClearCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Player))
        {
            PlayerElement.SetMediaPlayer(ViewModel.Player);
        }
    }

    private void OnPlayerElementDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var window = Frame.Tag as Window;
        if (window is null)
        {
            return;
        }

        if (window.AppWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
        {
            window.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
            MainSplitView.IsPaneOpen = ViewModel.OnlyOne;
        }
        else
        {
            MainSplitView.IsPaneOpen = false;
            window.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }

    private void OnPlayerElementTapped(object sender, TappedRoutedEventArgs e)
    {
        var position = e.GetPosition(PlayerElement);
        var rect = TransportControls.TransformToVisual(PlayerElement).TransformBounds(new Rect(0, 0, TransportControls.ActualWidth, TransportControls.ActualHeight));
        if (rect.Contains(position))
        {
            return;
        }

        ViewModel.PlayPauseCommand.Execute(default);
    }

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as WebDavStorageItemViewModel;
        ViewModel.PlayCommand.Execute(data);
    }
}

/// <summary>
/// WebDAV 播放器页面基类.
/// </summary>
public abstract class WebDavPlayerPageBase : PageBase<WebDavPlayerPageViewModel>
{
}
