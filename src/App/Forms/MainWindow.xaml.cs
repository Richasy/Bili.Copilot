﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 应用主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase
{
    private readonly AppViewModel _appViewModel = AppViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        CustomTitleBar.AttachedWindow = this;
        Width = 500;
        Height = 800;
        IsResizable = false;
        IsMaximizable = false;
        _appViewModel.NavigateRequest += OnAppViewModelNavigateRequest;
        _appViewModel.RequestShowTip += OnAppViewModelRequestShowTip;
        _appViewModel.RequestShowMessage += OnAppViewModelRequestShowMessageAsync;
        _appViewModel.RequestPlay += OnAppViewModelRequestPlay;
        _appViewModel.RequestPlaylist += OnAppViewModelRequestPlaylist;
        _appViewModel.RequestSearch += OnRequestSearch;
    }

    /// <summary>
    /// 当前窗口实例.
    /// </summary>
    public static MainWindow Instance { get; private set; }

    /// <summary>
    /// Display a tip message and close it after the specified delay.
    /// </summary>
    /// <param name="element">The element to insert.</param>
    /// <param name="delaySeconds">Delay time (seconds).</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        _ = TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }

    private async void OnTitleBarLoadedAsync(object sender, RoutedEventArgs e)
        => await _appViewModel.InitializeAsync();

    private void OnAppViewModelNavigateRequest(object sender, AppNavigationEventArgs e)
    {
        var pageType = e.PageId switch
        {
            PageType.Home => typeof(HomePage),
            PageType.Partition => typeof(VideoPartitionPage),
            PageType.Popular => typeof(PopularPage),
            PageType.Dynamic => typeof(DynamicPage),
            PageType.Live => typeof(LivePage),
            PageType.Anime => typeof(AnimePage),
            PageType.Film => typeof(FilmPage),
            PageType.Article => typeof(ArticlePage),
            PageType.Watchlist => typeof(WatchlistPage),
            PageType.SignIn => typeof(SignInPage),
            _ => throw new NotImplementedException(),
        };

        _ = MainFrame.Navigate(pageType, e.Parameter);
    }

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotificationEventArgs e)
        => new TipPopup(e.Message).ShowAsync(e.Type);

    private async void OnAppViewModelRequestShowMessageAsync(object sender, string e)
    {
        var dialog = new TipDialog(e)
        {
            XamlRoot = Content.XamlRoot,
        };
        _ = await dialog.ShowAsync();
    }

    private void OnAppViewModelRequestPlay(object sender, PlaySnapshot e)
    {
        var playWindow = new PlayerWindow(e);
        playWindow.Activate();
    }

    private void OnAppViewModelRequestPlaylist(object sender, List<VideoInformation> e)
    {
        var playWindow = new PlayerWindow(e);
        playWindow.Activate();
    }

    private void OnRequestSearch(object sender, string e)
    {
        Activate();
        if (MainFrame.Content is HomePage homePage)
        {
            homePage.ViewModel.OpenSearchCommand.Execute(e);
        }
        else
        {
            _appViewModel.Navigate(PageType.Home, e);
        }
    }

    private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        => _appViewModel.Navigate(PageType.Settings);
}
