// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Forms;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;
using WinRT.Interop;

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
        DependencyProperty.Register(nameof(BackButtonVisibility), typeof(Visibility), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnBackButtonVisibilityChanged)));

    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(AppTitleBar), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="HasBackground"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty HasBackgroundProperty =
        DependencyProperty.Register(nameof(HasBackground), typeof(bool), typeof(AppTitleBar), new PropertyMetadata(true));

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
    /// 是否有背景.
    /// </summary>
    public bool HasBackground
    {
        get => (bool)GetValue(HasBackgroundProperty);
        set => SetValue(HasBackgroundProperty, value);
    }

    private static void OnBackButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as AppTitleBar;
        instance.SetDragRegion();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => SetDragRegion();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => SetDragRegion();

    private void SetDragRegion()
    {
        if (AttachedWindow.AppWindow == null || !AppWindowTitleBar.IsCustomizationSupported())
        {
            return;
        }

        var windowHandle = WindowNative.GetWindowHandle(AttachedWindow);
        var scaleFactor = Windows.Win32.PInvoke.GetDpiForWindow(new Windows.Win32.Foundation.HWND(windowHandle)) / 96d;
        var leftInset = AttachedWindow.AppWindow.TitleBar.LeftInset;
        var leftPadding = BackButtonVisibility == Visibility.Visible ? 48 * scaleFactor : 0;
        var dragRect = default(RectInt32);
        dragRect.X = Convert.ToInt32(leftInset + leftPadding);
        dragRect.Y = 0;
        dragRect.Height = Convert.ToInt32(ActualHeight * scaleFactor);
        dragRect.Width = Convert.ToInt32((ActualWidth * scaleFactor) - leftPadding);
        AttachedWindow.AppWindow.TitleBar.SetDragRectangles(new[] { dragRect });
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => BackButtonClick?.Invoke(this, EventArgs.Empty);
}
