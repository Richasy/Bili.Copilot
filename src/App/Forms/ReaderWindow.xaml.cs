// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using CommunityToolkit.WinUI.UI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 文章阅读器页面.
/// </summary>
public sealed partial class ReaderWindow : WindowBase
{
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderWindow"/> class.
    /// </summary>
    public ReaderWindow(ArticleInformation info)
    {
        InitializeComponent();
        var title = string.Format(ResourceToolkit.GetLocalizedString(StringNames.ArticleReader), info.Identifier.Title);
        CustomTitleBar.Title = title;
        Title = title;
        CustomTitleBar.AttachedWindow = this;
        CustomTitleBar.BackButtonVisibility = Visibility.Collapsed;
        var theme = new ThemeListener(DispatcherQueue);
        var color = theme.CurrentTheme == ApplicationTheme.Dark ? AppToolkit.HexToColor("#2f3239") : AppToolkit.HexToColor("#faf9fa");
        CustomTitleBar.Background = new SolidColorBrush(color);
        MainFrame.Background = new SolidColorBrush(color);
        Width = 500;
        Height = MainWindow.Instance.Height;
        MinWidth = MainWindow.Instance.MinWidth;
        MinHeight = MainWindow.Instance.MinHeight;
        MainFrame.Navigate(typeof(ReaderPage), info);
        Activated += OnActivated;
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
