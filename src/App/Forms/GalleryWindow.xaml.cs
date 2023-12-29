// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 图片浏览窗口.
/// </summary>
public sealed partial class GalleryWindow : WindowBase
{
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryWindow"/> class.
    /// </summary>
    public GalleryWindow(ShowImageEventArgs args)
    {
        InitializeComponent();
        MainFrame.Tag = this;
        Activated += OnActivated;
        Closed += OnClosed;
        Title = ResourceToolkit.GetLocalizedString(StringNames.ImageGallery);
        MinWidth = 500;
        MinHeight = 500;
        _ = MainFrame.Navigate(typeof(GalleryPage), args);

        MoveAndResize();
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.GalleryWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.GalleryWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (!_isActivated)
        {
            var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsGalleryWindowMaximized, false);
            if (isMaximized)
            {
                (AppWindow.Presenter as OverlappedPresenter).Maximize();
            }

            var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
            AppViewModel.Instance.ChangeTheme(localTheme);
        }

        _isActivated = true;
    }

    private void OnClosed(object sender, WindowEventArgs args)
        => SaveCurrentWindowStats();

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.GalleryWindowWidth, 800d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.GalleryWindowHeight, 600d);
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
        var isMaximized = PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsGalleryWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.GalleryWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.GalleryWindowPositionTop, top);
            SettingsToolkit.WriteLocalSetting(SettingNames.GalleryWindowHeight, Height < 500 ? 500d : Height);
            SettingsToolkit.WriteLocalSetting(SettingNames.GalleryWindowWidth, Width);
        }
    }
}
