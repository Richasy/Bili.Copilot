// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Richasy.WinUI.Share.Base;
using WinUIEx;

namespace BiliCopilot.UI;

/// <summary>
/// 主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SetTitleBar(RootLayout.GetMainTitleBar());
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/Logo.ico");
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        MinWidth = 640;
        MinHeight = 480;
    }
}
