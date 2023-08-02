// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = SettingsPageViewModel.Instance;
    }
}

/// <summary>
/// <see cref="SettingsPage"/> 的基类.
/// </summary>
public abstract class SettingsPageBase : PageBase<SettingsPageViewModel>
{
}
