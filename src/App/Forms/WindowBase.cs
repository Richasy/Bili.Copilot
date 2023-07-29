// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// Window base class.
/// </summary>
public class WindowBase : WindowEx
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowBase"/> class.
    /// </summary>
    public WindowBase()
    {
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");
    }
}
