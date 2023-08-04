// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml;
using WinUIEx;

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
        Title = ResourceToolkit.GetLocalizedString(StringNames.ImageGallery);
        Width = 800;
        Height = 600;
        MinWidth = 560;
        MinHeight = 320;
        MainFrame.Navigate(typeof(GalleryPage), args);
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        this.CenterOnScreen();
        _isActivated = true;
    }
}
