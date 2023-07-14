// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.App.Forms;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 应用标题栏.
/// </summary>
public sealed partial class AppTitleBar : AppTitleBarBase
{
    /// <summary>
    /// Dependency property for <see cref="AttachedWindow"/>.
    /// </summary>
    public static readonly DependencyProperty AttachedWindowProperty =
        DependencyProperty.Register(nameof(AttachedWindow), typeof(WindowBase), typeof(AppTitleBar), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar()
    {
        InitializeComponent();
        ViewModel = AppViewModel.Instance;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// 附加的窗口对象.
    /// </summary>
    public WindowBase AttachedWindow
    {
        get => (WindowBase)GetValue(AttachedWindowProperty);
        set => SetValue(AttachedWindowProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetDragRegion();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => SetDragRegion();

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsBackButtonShown))
        {
            SetDragRegion();
        }
    }

    private void SetDragRegion()
    {
        if (AttachedWindow.AppWindow == null || !AppWindowTitleBar.IsCustomizationSupported())
        {
            return;
        }

        var windowHandle = WindowNative.GetWindowHandle(AttachedWindow);
        var scaleFactor = Windows.Win32.PInvoke.GetDpiForWindow(new Windows.Win32.Foundation.HWND(windowHandle)) / 96d;
        var leftInset = AttachedWindow.AppWindow.TitleBar.LeftInset;
        var leftPadding = ViewModel.IsBackButtonShown ? 48 * scaleFactor : 0;
        var dragRect = default(RectInt32);
        dragRect.X = Convert.ToInt32(leftInset + leftPadding);
        dragRect.Y = 0;
        dragRect.Height = Convert.ToInt32(ActualHeight * scaleFactor);
        dragRect.Width = Convert.ToInt32((ActualWidth * scaleFactor) - leftPadding);
        AttachedWindow.AppWindow.TitleBar.SetDragRectangles(new[] { dragRect });
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.BackCommand.Execute(default);
}

/// <summary>
/// <see cref="AppTitleBar"/> 的基类.
/// </summary>
public abstract class AppTitleBarBase : ReactiveUserControl<AppViewModel>
{
}
