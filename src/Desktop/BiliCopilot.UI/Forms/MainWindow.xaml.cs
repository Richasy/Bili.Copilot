// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Windowing;
using Richasy.WinUI.Share.Base;
using Windows.Graphics;
using WinUIEx;

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase
{
    private const int WindowMinWidth = 640;
    private const int WindowMinHeight = 480;
    private bool _isFirstActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SetTitleBar(RootLayout.GetMainTitleBar());
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        MinWidth = WindowMinWidth;
        MinHeight = WindowMinHeight;
        GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Add(this);

        Activated += OnActivated;
        Closed += OnClosed;
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            this.Get<AppViewModel>().ActivatedWindow = this;
        }

        if (!_isFirstActivated)
        {
            return;
        }

        MoveAndResize();
        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsMainWindowMaximized, false);
        if (isMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter).Maximize();
        }

        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        this.Get<AppViewModel>().ChangeThemeCommand.Execute(localTheme);
        _isFirstActivated = false;
    }

    private void OnClosed(object sender, WindowEventArgs e)
    {
        foreach (var item in this.Get<AppViewModel>().Windows)
        {
            if (item is not MainWindow)
            {
                item.Close();
            }
        }

        Activated -= OnActivated;
        Closed -= OnClosed;

        GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Remove(this);
        SaveCurrentWindowStats();
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowHeight, 740d);
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
        AppWindow.MoveAndResize(rect);
    }

    private void SaveCurrentWindowStats()
    {
        var left = AppWindow.Position.X;
        var top = AppWindow.Position.Y;
        var isMaximized = PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsMainWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionTop, top);

            if (Height >= WindowMinHeight && Width >= WindowMinWidth)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowHeight, Height);
                SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowWidth, Width);
            }
        }
    }
}
