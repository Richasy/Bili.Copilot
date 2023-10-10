// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 应用主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase
{
    private readonly AppViewModel _appViewModel = AppViewModel.Instance;
    private readonly IActivatedEventArgs _launchArgs;
    private bool _isInitialized;
    private string _currentPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(IActivatedEventArgs args = default)
    {
        InitializeComponent();
        _launchArgs = args;
        Instance = this;
        SetTitleBar(CustomTitleBar);
        CustomTitleBar.AttachedWindow = this;
        _appViewModel.NavigateRequest += OnAppViewModelNavigateRequest;
        _appViewModel.RequestShowTip += OnAppViewModelRequestShowTip;
        _appViewModel.RequestShowMessage += OnAppViewModelRequestShowMessageAsync;
        _appViewModel.RequestShowUpdateDialog += OnAppViewModelRequestShowUpdateDialogAsync;
        _appViewModel.RequestPlay += OnAppViewModelRequestPlay;
        _appViewModel.RequestPlaylist += OnAppViewModelRequestPlaylist;
        _appViewModel.RequestSearch += OnRequestSearch;
        _appViewModel.RequestShowUserSpace += OnRequestShowUserSpace;
        _appViewModel.RequestShowCommentWindow += OnRequestShowCommentWindow;
        _appViewModel.ActiveMainWindow += OnActiveMainWindow;
        _appViewModel.RequestRead += OnRequestRead;
        _appViewModel.RequestSummarizeVideoContent += OnRequestSummarizeVideoContentAsync;
        _appViewModel.RequestSummarizeArticleContent += OnRequestSummarizeArticleContentAsync;
        _appViewModel.RequestEvaluateVideo += OnRequestEvaluateVideoAsync;
        _appViewModel.RequestShowImages += OnRequestShowImages;

        RootGrid.SizeChanged += OnRootGridSizeChanged;
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

    /// <summary>
    /// 处理激活事件.
    /// </summary>
    /// <param name="e">激活事件参数.</param>
    public void ActivateArgumentsAsync(IActivatedEventArgs e = default)
    {
        e ??= _launchArgs;
        if (e.Kind == ActivationKind.Protocol)
        {
            // TODO: Handle protocol activation.
        }
    }

    /// <summary>
    /// 更改菜单布局.
    /// </summary>
    public void CheckMenuLayout()
    {
        var position = CustomTitleBar.ActualWidth > 900 ? "left" : "bottom";
        if (_currentPosition == position || !_isInitialized)
        {
            return;
        }

        _currentPosition = position;

        if (position == "bottom")
        {
            Grid.SetRow(NavContainer, 2);
            Grid.SetRowSpan(NavContainer, 1);
            Grid.SetColumn(NavContainer, 1);
            NavContainer.Padding = new Thickness(4, 0, 4, 0);
            NavContainer.Height = 56;
            MainNavView.Height = 48;
            NavContainer.Width = double.NaN;
            MainNavView.Width = double.NaN;
            MainNavView.Margin = new Thickness(0, -4, 0, 0);
            MainNavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
            MainFrame.BorderThickness = new Thickness(0, 0, 0, 1);
            CustomTitleBar.IsCompact = true;
            MainNavView.IsPaneOpen = false;
            MainFrame.CornerRadius = new CornerRadius(0, 0, 0, 0);
            MainFrame.Padding = new Thickness(0, 12, 0, 0);
            SettingIconButton.Visibility = Visibility.Visible;
            SettingFullButton.Visibility = Visibility.Collapsed;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;

            _appViewModel.PagePadding = 20;
            _appViewModel.HeaderFontSize = 20;
            foreach (var item in _appViewModel.NavigateItems)
            {
                item.DisplayTitle = default;
            }
        }
        else
        {
            Grid.SetRow(NavContainer, 1);
            Grid.SetRowSpan(NavContainer, 2);
            Grid.SetColumn(NavContainer, 0);
            NavContainer.Padding = new Thickness(0, 0, 0, 0);
            NavContainer.Height = double.NaN;
            MainNavView.Height = double.NaN;
            NavContainer.Width = 240;
            MainNavView.Width = 240;
            MainNavView.Margin = new Thickness(0);
            MainNavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
            MainNavView.IsPaneOpen = true;
            MainFrame.BorderThickness = new Thickness(1, 1, 0, 0);
            CustomTitleBar.IsCompact = false;
            MainFrame.CornerRadius = new CornerRadius(8, 0, 0, 0);
            MainFrame.Padding = new Thickness(0, 30, 0, 0);
            SettingIconButton.Visibility = Visibility.Collapsed;
            SettingFullButton.Visibility = Visibility.Visible;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            _appViewModel.PagePadding = 44;
            _appViewModel.HeaderFontSize = 28;
            foreach (var item in _appViewModel.NavigateItems)
            {
                item.DisplayTitle = item.Data.Title;
            }
        }
    }

    private void OnTitleBarLoaded(object sender, RoutedEventArgs e)
    {
        if (_isInitialized)
        {
            return;
        }

        DispatcherQueue.TryEnqueue(async () =>
        {
            await _appViewModel.InitializeAsync();

            if (_launchArgs != null)
            {
                ActivateArgumentsAsync();
            }

#if !DEBUG
            _appViewModel.CheckUpdateCommand.Execute(default);
#endif
            _appViewModel.CheckAIFeatureCommand.Execute(default);
            _appViewModel.CheckBBDownExistCommand.Execute(default);

            _isInitialized = true;

            CheckMenuLayout();
        });
    }

    private void OnAppViewModelNavigateRequest(object sender, AppNavigationEventArgs e)
    {
        Activate();
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
            PageType.Settings => typeof(SettingsPage),
            _ => throw new NotImplementedException(),
        };

        TraceLogger.LogMainPageNavigation(e.PageId.ToString());
        _ = MainFrame.Navigate(pageType, e.Parameter);
    }

    private void OnRootGridSizeChanged(object sender, SizeChangedEventArgs e)
        => CheckMenuLayout();

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotification e)
        => new TipPopup(e.Message).ShowAsync(e.Type);

    private async void OnAppViewModelRequestShowMessageAsync(object sender, string e)
    {
        var dialog = new TipDialog(e)
        {
            XamlRoot = Content.XamlRoot,
        };
        _ = await dialog.ShowAsync();
    }

    private async void OnAppViewModelRequestShowUpdateDialogAsync(object sender, UpdateEventArgs e)
    {
        var dialog = new UpdateDialog(e)
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

    private void OnBackButtonClick(object sender, EventArgs e)
        => _appViewModel.BackCommand.Execute(default);

    private void OnRequestShowUserSpace(object sender, UserProfile e)
    {
        var window = new UserSpaceWindow(e);
        window.Activate();
    }

    private void OnRequestShowCommentWindow(object sender, ShowCommentEventArgs e)
    {
        var window = new CommentWindow(e);
        window.Activate();
    }

    private void OnRequestRead(object sender, ArticleInformation e)
    {
        var window = new ReaderWindow(e);
        window.Activate();
    }

    private void OnRequestShowImages(object sender, ShowImageEventArgs e)
    {
        var window = new GalleryWindow(e);
        window.Activate();
    }

    private void OnActiveMainWindow(object sender, EventArgs e)
        => Activate();

    private async void OnRequestSummarizeVideoContentAsync(object sender, VideoIdentifier e)
        => await ShowAIDialogAsync(e, AIFeatureType.VideoSummarize);

    private async void OnRequestEvaluateVideoAsync(object sender, VideoIdentifier e)
        => await ShowAIDialogAsync(e, AIFeatureType.VideoEvaluation);

    private async void OnRequestSummarizeArticleContentAsync(object sender, ArticleIdentifier e)
        => await ShowAIDialogAsync(e, AIFeatureType.ArticleSummarize);

    private async Task ShowAIDialogAsync(object data, AIFeatureType type)
    {
        var dialog = new AIFeatureDialog(data, type)
        {
            XamlRoot = MainFrame.XamlRoot,
        };
        await dialog.ShowAsync();
    }
}
