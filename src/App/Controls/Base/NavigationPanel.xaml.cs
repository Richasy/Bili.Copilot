// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 导航面板.
/// </summary>
public sealed partial class NavigationPanel : NavigationPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPanel"/> class.
    /// </summary>
    public NavigationPanel()
    {
        InitializeComponent();
        ViewModel = AppViewModel.Instance;
    }
}

/// <summary>
/// 导航面板基类.
/// </summary>
public abstract class NavigationPanelBase : ReactiveUserControl<AppViewModel>
{
}
