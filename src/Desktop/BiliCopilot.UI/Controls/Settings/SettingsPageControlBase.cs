// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 设置页面控件基类.
/// </summary>
public abstract class SettingsPageControlBase : LayoutUserControlBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageControlBase"/> class.
    /// </summary>
    protected SettingsPageControlBase() => ViewModel = this.Get<SettingsPageViewModel>();
}
