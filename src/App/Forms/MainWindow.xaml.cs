// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;
using Bili.Copilot.ViewModels.Views;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Windows.System;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 应用主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase, ITipWindow, IUserSpaceWindow
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
        _appViewModel.RequestPlayWebDav += OnAppViewModelRequestPlayWebDav;
        _appViewModel.RequestSearch += OnRequestSearch;
        _appViewModel.RequestShowFans += OnRequestShowFans;
        _appViewModel.RequestShowFollows += OnRequestShowFollows;
        _appViewModel.RequestShowMyMessages += OnRequestShowMyMessages;
        _appViewModel.RequestShowUserSpace += OnRequestShowUserSpace;
        _appViewModel.RequestShowViewLater += OnRequestShowViewLater;
        _appViewModel.RequestShowHistory += OnRequestShowHistory;
        _appViewModel.RequestShowFavorites += OnRequestShowFavorites;
        _appViewModel.RequestShowCommentWindow += OnRequestShowCommentWindow;
        _appViewModel.ActiveMainWindow += OnActiveMainWindow;
        _appViewModel.RequestRead += OnRequestRead;
        _appViewModel.RequestShowImages += OnRequestShowImages;
        _appViewModel.BackRequest += OnBackRequestedAsync;

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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void ShowUserSpace(UserProfile profile)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        var userVM = new UserSpaceViewModel();
        userVM.SetUserProfile(profile);
        _ = SplitFrame.Navigate(typeof(UserSpacePage), userVM);
    }

    internal static Visibility IsMainContentShown(bool isOverlayShown)
        => isOverlayShown ? Visibility.Collapsed : Visibility.Visible;

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private static PlayerWindow GetPlayerWindow()
    {
        var playerBehavior = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior.Main);
        PlayerWindow window = default;
        if (playerBehavior == PlayerWindowBehavior.Single)
        {
            window = AppViewModel.Instance.DisplayWindows.OfType<PlayerWindow>().FirstOrDefault();
        }

        if (window == null)
        {
            window = new PlayerWindow();
        }

        return window;
    }

    private static bool IsMainWindowPlayer()
        => SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior.Main) == PlayerWindowBehavior.Main;

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
            PageType.WebDav => typeof(WebDavPage),
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
        if (IsMainWindowPlayer())
        {
            BeforeEnterPlayerPageAsync();
            PlayerUtils.InitializePlayer(e, OverlayFrame, this);
        }
        else
        {
            GetPlayerWindow().SetData(e);
        }
    }

    private void OnAppViewModelRequestPlaylist(object sender, List<VideoInformation> e)
    {
        if (IsMainWindowPlayer())
        {
            BeforeEnterPlayerPageAsync();
            PlayerUtils.InitializePlayer(e, OverlayFrame, this);
        }
        else
        {
            GetPlayerWindow().SetData(e);
        }
    }

    private void OnAppViewModelRequestPlayWebDav(object sender, List<WebDavStorageItemViewModel> e)
    {
        if (IsMainWindowPlayer())
        {
            BeforeEnterPlayerPageAsync();
            PlayerUtils.InitializePlayer(e, OverlayFrame, this);
        }
        else
        {
            var title = e.Count == 1
            ? e.First().Data.DisplayName
            : Uri.UnescapeDataString(SettingsToolkit.ReadLocalSetting(SettingNames.WebDavLastPath, "/").Split("/", StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? ResourceToolkit.GetLocalizedString(StringNames.RootDirectory));
            GetPlayerWindow().SetData(e, title);
        }
    }

    private void OnRequestSearch(object sender, string e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not SearchPage)
        {
            var vm = new SearchDetailViewModel();
            vm.SetKeyword(e);
            vm.InitializeCommand.Execute(default);
            _ = OverlayFrame.Navigate(typeof(SearchPage), vm);
        }
        else
        {
            var page = OverlayFrame.Content as SearchPage;
            page.ViewModel.SetKeyword(e);
            page.ViewModel.InitializeCommand.Execute(default);
        }
    }

    private void OnRequestShowMyMessages(object sender, EventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not MessagePage)
        {
            _ = OverlayFrame.Navigate(typeof(MessagePage), _appViewModel.Message);
        }
    }

    private void OnRequestShowFollows(object sender, EventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not FollowsPage)
        {
            _ = OverlayFrame.Navigate(typeof(FollowsPage), _appViewModel.Follows);
        }
    }

    private void OnRequestShowFans(object sender, UserProfile e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not FansPage)
        {
            _ = OverlayFrame.Navigate(typeof(FansPage), _appViewModel.Fans);
        }
    }

    private void OnRequestShowFavorites(object sender, FavoriteType e)
    {
        Activate();
        MainSplitView.IsPaneOpen = false;

        if (OverlayFrame.Content is not FavoritesPage)
        {
            _ = OverlayFrame.Navigate(typeof(FavoritesPage), e);
        }
        else
        {
            FavoritesPageViewModel.Instance.Type = e;
        }
    }

    private void OnRequestShowHistory(object sender, EventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        SplitFrame.Navigate(typeof(HistoryPage));
    }

    private void OnRequestShowViewLater(object sender, EventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        SplitFrame.Navigate(typeof(ViewLaterPage));
    }

    private async void OnBackButtonClickAsync(object sender, EventArgs e)
    {
        _appViewModel.BackCommand.Execute(default);
        if (OverlayFrame.Content is not null)
        {
            _ = OverlayFrame.Navigate(typeof(Page));

            await Task.Delay(200);
            OverlayFrame.Content = null;
        }
    }

    private void OnRequestShowUserSpace(object sender, UserProfile e)
    {
        if (AppViewModel.Instance.ActivatedWindow is IUserSpaceWindow window)
        {
            window.ShowUserSpace(e);
        }
        else
        {
            ShowUserSpace(e);
        }
    }

    private void OnRequestShowCommentWindow(object sender, ShowCommentEventArgs e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        var vm = new CommentModuleViewModel();
        vm.SetData(e.SourceId, e.Type);
        _ = SplitFrame.Navigate(typeof(CommentPage), vm);
    }

    private void OnRequestRead(object sender, ArticleInformation e)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        _ = SplitFrame.Navigate(typeof(ReaderPage), e);
    }

    private void OnRequestShowImages(object sender, ShowImageEventArgs e)
    {
        var window = new GalleryWindow(e);
        window.Activate();
    }

    private async void OnBackRequestedAsync(object sender, EventArgs e)
    {
        if (OverlayFrame.Content.ToString().Contains("PlayerPage"))
        {
            MinWidth = 800;
            MinHeight = 640;
            OverlayFrame.Navigate(typeof(Page));
            OverlayFrame.Content = default;
            _appViewModel.IsTitleBarShown = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            await Task.Delay(200);
            CustomTitleBar.Refresh();
        }
    }

    private void OnActiveMainWindow(object sender, EventArgs e)
        => Activate();

    private void OnClosed(object sender, WindowEventArgs args)
    {
        foreach (var item in AppViewModel.Instance.DisplayWindows.ToArray())
        {
            if (item is not MainWindow)
            {
                item.Close();
            }
        }

        if (_appViewModel.IsOverlayShown && OverlayFrame.Content is not null)
        {
            _appViewModel.BackCommand.Execute(default);
        }

        SaveCurrentWindowStats();
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        KeyboardHook.KeyDown -= OnWindowKeyDown;
        KeyboardHook.Stop();
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            KeyboardHook.Start();
            KeyboardHook.KeyDown += OnWindowKeyDown;
        }

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
        else
        {
            AccountViewModel.Instance.InitialCommunityInformationCommand.Execute(default);
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

    private async void SetNormalDragAreaAsync()
    {
        await Task.Delay(200);
        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(Win32Interop.GetWindowIdFromWindow(this.GetWindowHandle()));
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, new RectInt32[] { });
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
        if (AppWindow is null)
        {
            return;
        }

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

    private async void BeforeEnterPlayerPageAsync()
    {
        MainSplitView.IsPaneOpen = false;
        _appViewModel.IsOverlayShown = true;
        _appViewModel.IsTitleBarShown = false;
        MinWidth = 560;
        MinHeight = 320;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
        await Task.Delay(200);
        SetNormalDragAreaAsync();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_appViewModel.IsOverlayShown && !_appViewModel.IsTitleBarShown)
        {
            SetNormalDragAreaAsync();
        }
    }

    private void OnWindowKeyDown(object sender, PlayerKeyboardEventArgs e)
    {
        if ((e.Key == VirtualKey.Space || e.Key == VirtualKey.Pause) && _appViewModel.IsOverlayShown)
        {
            if (e.Key == VirtualKey.Space)
            {
                var focusEle = FocusManager.GetFocusedElement(MainFrame.XamlRoot);
                if (focusEle is TextBox)
                {
                    return;
                }
            }

            e.Handled = true;
            if (OverlayFrame.Content is VideoPlayerPageBase page)
            {
                page.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
            else if (OverlayFrame.Content is LivePlayerPage livePage)
            {
                livePage.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
            else if (OverlayFrame.Content is PgcPlayerPage pgcPage)
            {
                pgcPage.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
            else if (OverlayFrame.Content is WebDavPlayerPage webDavPage)
            {
                webDavPage.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
        }
    }
}
