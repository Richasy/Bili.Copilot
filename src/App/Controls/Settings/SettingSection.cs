// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 设置区块.
/// </summary>
public abstract class SettingSection : ReactiveUserControl<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingSection"/> class.
    /// </summary>
    public SettingSection() => ViewModel = SettingsPageViewModel.Instance;
}
