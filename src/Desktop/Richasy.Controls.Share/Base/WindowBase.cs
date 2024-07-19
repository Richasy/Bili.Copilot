// Copyright (c) Bili Copilot. All rights reserved.

using WinUIEx;

namespace Richasy.WinUI.Share.Base;

/// <summary>
/// 窗口基类.
/// </summary>
public abstract class WindowBase : WindowEx
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowBase"/> class.
    /// </summary>
    protected WindowBase()
    {
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.IconShowOptions = Microsoft.UI.Windowing.IconShowOptions.HideIconAndSystemMenu;
        SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
    }
}
