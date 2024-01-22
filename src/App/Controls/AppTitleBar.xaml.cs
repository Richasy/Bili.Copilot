// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Forms;
using Bili.Copilot.Libs.Toolkit;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 应用标题栏.
/// </summary>
public sealed partial class AppTitleBar : UserControl
{
    /// <summary>
    /// <see cref="AttachedWindow"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AttachedWindowProperty =
        DependencyProperty.Register(nameof(AttachedWindow), typeof(WindowBase), typeof(AppTitleBar), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="BackButtonVisibility"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty BackButtonVisibilityProperty =
        DependencyProperty.Register(nameof(BackButtonVisibility), typeof(Visibility), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnBackButtonVisibilityChangedAsync)));

    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(AppTitleBar), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// 后退按钮点击事件.
    /// </summary>
    public event EventHandler BackButtonClick;

    /// <summary>
    /// 附加的窗口对象.
    /// </summary>
    public WindowBase AttachedWindow
    {
        get => (WindowBase)GetValue(AttachedWindowProperty);
        set => SetValue(AttachedWindowProperty, value);
    }

    /// <summary>
    /// 回退按钮可见性.
    /// </summary>
    public Visibility BackButtonVisibility
    {
        get => (Visibility)GetValue(BackButtonVisibilityProperty);
        set => SetValue(BackButtonVisibilityProperty, value);
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 刷新可拖拽区域.
    /// </summary>
    public void Refresh()
    {
        if (IsLoaded)
        {
            SetDragRegion();
        }
    }

    private static async void OnBackButtonVisibilityChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;

        await Task.Delay(200);
        instance.SetDragRegion();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => SetDragRegion();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => SetDragRegion();

    private void SetDragRegion()
    {
        if (AttachedWindow.AppWindow == null || !AppWindowTitleBar.IsCustomizationSupported())
        {
            return;
        }

        var scaleFactor = AttachedWindow.GetDpiForWindow() / 96d;
        var transform = BackButton.TransformToVisual(default);
        var backButtonBounds = transform.TransformBounds(new Rect(0, 0, BackButton.ActualWidth, BackButton.ActualHeight));
        var backButtonRect = AppToolkit.GetRectInt32(backButtonBounds, scaleFactor);

        transform = SearchModule.TransformToVisual(default);
        var searchBounds = transform.TransformBounds(new Rect(0, 0, SearchModule.ActualWidth, SearchModule.ActualHeight));
        var searchBoxRect = AppToolkit.GetRectInt32(searchBounds, scaleFactor);

        transform = FixModule.TransformToVisual(default);
        var fixBounds = transform.TransformBounds(new Rect(0, 0, FixModule.ActualWidth, FixModule.ActualHeight));
        var fixRect = AppToolkit.GetRectInt32(fixBounds, scaleFactor);

        transform = AccountModule.TransformToVisual(default);
        var accountBounds = transform.TransformBounds(new Rect(0, 0, AccountModule.ActualWidth, AccountModule.ActualHeight));
        var accountRect = AppToolkit.GetRectInt32(accountBounds, scaleFactor);

        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(Win32Interop.GetWindowIdFromWindow(AttachedWindow.GetWindowHandle()));
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, new RectInt32[] { backButtonRect, searchBoxRect, fixRect, accountRect });
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => BackButtonClick?.Invoke(this, EventArgs.Empty);

    private async void OnFixModuleVisibilityChangedAsync(object sender, EventArgs e)
    {
        await Task.Delay(200);
        SetDragRegion();
    }
}
