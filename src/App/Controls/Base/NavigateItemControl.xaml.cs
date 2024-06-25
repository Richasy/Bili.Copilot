// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 导航项控件.
/// </summary>
public sealed partial class NavigateItemControl : NavigateItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemControl"/> class.
    /// </summary>
    public NavigateItemControl() => InitializeComponent();

    private void OnNavItemClick(object sender, RoutedEventArgs e)
    {
        if (AppViewModel.Instance.CurrentPage == ViewModel?.Data.Id)
        {
            return;
        }

        AppViewModel.Instance.Navigate(ViewModel?.Data?.Id ?? Models.Constants.App.PageType.Popular);
    }
}

/// <summary>
/// 导航项控件基类.
/// </summary>
public abstract class NavigateItemControlBase : ReactiveUserControl<NavigateItemViewModel>
{
}
