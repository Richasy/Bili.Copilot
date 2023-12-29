// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 应用主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase, ITipWindow
{
    private readonly AppViewModel _appViewModel = AppViewModel.Instance;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        SetTitleBar(CustomTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        CustomTitleBar.AttachedWindow = this;
        _appViewModel.NavigateRequest += OnAppViewModelNavigateRequest;
        _appViewModel.RequestShowTip += OnAppViewModelRequestShowTip;
        _appViewModel.RequestShowMessage += OnAppViewModelRequestShowMessageAsync;
        _appViewModel.RequestShowUpdateDialog += OnAppViewModelRequestShowUpdateDialogAsync;
        _appViewModel.RequestPlay += OnAppViewModelRequestPlay;
        _appViewModel.RequestPlaylist += OnAppViewModelRequestPlaylist;
        _appViewModel.RequestSearch += OnRequestSearch;
        _appViewModel.RequestShowFans += OnRequestShowFans;
        _appViewModel.RequestShowFollows += OnRequestShowFollows;
        _appViewModel.RequestShowMyMessages += OnRequestShowMyMessages;
        _appViewModel.RequestShowUserSpace += OnRequestShowUserSpace;
        _appViewModel.RequestShowCommentWindow += OnRequestShowCommentWindow;
        _appViewModel.ActiveMainWindow += OnActiveMainWindow;
        _appViewModel.RequestRead += OnRequestRead;
        _appViewModel.RequestSummarizeVideoContent += OnRequestSummarizeVideoContentAsync;
        _appViewModel.RequestSummarizeArticleContent += OnRequestSummarizeArticleContentAsync;
        _appViewModel.RequestEvaluateVideo += OnRequestEvaluateVideoAsync;
        _appViewModel.RequestShowImages += OnRequestShowImages;

        MinWidth = 800;
        MinHeight = 640;

        Activated += OnActivated;
        Closed += OnClosed;

        MoveAndResize();
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

    internal static Visibility IsMainContentShown(bool isOverlayShown)
        => isOverlayShown ? Visibility.Collapsed : Visibility.Visible;

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
        return new PointInt32(left, top);
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

#if !DEBUG
            _appViewModel.CheckUpdateCommand.Execute(default);
#endif
            _appViewModel.CheckAIFeatureCommand.Execute(default);
            _appViewModel.CheckBBDownExistCommand.Execute(default);

            _isInitialized = true;
        });
    }

    private void OnAppViewModelNavigateRequest(object sender, AppNavigationEventArgs e)
    {
        Activate();
        var pageType = e.PageId switch
        {
            PageType.Partition => typeof(VideoPartitionPage),
            PageType.Popular => typeof(PopularPage),
            PageType.Dynamic => typeof(DynamicPage),
            PageType.Live => typeof(LivePage),
            PageType.Anime => typeof(AnimePage),
            PageType.Film => typeof(FilmPage),
            PageType.Article => typeof(ArticlePage),
            PageType.Settings => typeof(SettingsPage),
            _ => throw new NotImplementedException(),
        };

        TraceLogger.LogMainPageNavigation(e.PageId.ToString());
        _ = MainFrame.Navigate(pageType, e.Parameter);
    }

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
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not SearchPage)
        {
            OverlayFrame.Navigate(typeof(SearchPage), _appViewModel.Search);
        }
    }

    private void OnRequestShowMyMessages(object sender, EventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not MessagePage)
        {
            OverlayFrame.Navigate(typeof(MessagePage), _appViewModel.Message);
        }
    }

    private void OnRequestShowFollows(object sender, EventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not FollowsPage)
        {
            OverlayFrame.Navigate(typeof(FollowsPage), _appViewModel.Follows);
        }
    }

    private void OnRequestShowFans(object sender, UserProfile e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not FansPage)
        {
            OverlayFrame.Navigate(typeof(FansPage), _appViewModel.Fans);
        }
    }

    private async void OnBackButtonClickAsync(object sender, EventArgs e)
    {
        _appViewModel.BackCommand.Execute(default);
        if (OverlayFrame.Content is not null)
        {
            OverlayFrame.Navigate(typeof(Page));

            await Task.Delay(200);
            OverlayFrame.Content = null;
        }
    }

    private void OnRequestShowUserSpace(object sender, UserProfile e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        var userVM = new UserSpaceViewModel();
        userVM.SetUserProfile(e);
        SplitFrame.Navigate(typeof(UserSpacePage), userVM);
    }

    private void OnRequestShowCommentWindow(object sender, ShowCommentEventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        var vm = new CommentModuleViewModel();
        vm.SetData(e.SourceId, e.Type);
        SplitFrame.Navigate(typeof(CommentPage), vm);
    }

    private void OnRequestRead(object sender, ArticleInformation e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        SplitFrame.Navigate(typeof(ReaderPage), e);
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

    private void OnClosed(object sender, WindowEventArgs args)
        => SaveCurrentWindowStats();

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (!_isInitialized)
        {
            var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsMainWindowMaximized, false);
            if (isMaximized)
            {
                (AppWindow.Presenter as OverlappedPresenter).Maximize();
            }

            var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
            AppViewModel.Instance.ChangeTheme(localTheme);
        }
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowWidth, 1000d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowHeight, 700d);
        var width = Convert.ToInt32(previousWidth * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);

        // Ensure the window is not larger than the work area.
        if (height > workArea.Height - 20)
        {
            height = workArea.Height - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y;
        var left = isZeroPoint || !isValidPosition
            ? (workArea.Width - width) / 2d
            : lastPoint.X;
        var top = isZeroPoint || !isValidPosition
            ? (workArea.Height - height) / 2d
            : lastPoint.Y;
        return new RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private void MoveAndResize()
    {
        var lastPoint = GetSavedWindowPosition();
        var areas = DisplayArea.FindAll();
        var workArea = default(RectInt32);
        for (var i = 0; i < areas.Count; i++)
        {
            var area = areas[i];
            if (area.WorkArea.X < lastPoint.X && area.WorkArea.X + area.WorkArea.Width > lastPoint.X)
            {
                workArea = area.WorkArea;
                break;
            }
        }

        if (workArea == default)
        {
            workArea = DisplayArea.Primary.WorkArea;
        }

        var rect = GetRenderRect(workArea);
        MinWidth = 800;
        MinHeight = 640;

        AppWindow.MoveAndResize(rect);
    }

    private void SaveCurrentWindowStats()
    {
        var left = AppWindow.Position.X;
        var top = AppWindow.Position.Y;
        var isMaximized = Windows.Win32.PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsMainWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionTop, top);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowHeight, Height < 640 ? 640d : Height);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowWidth, Width);
        }
    }
}
