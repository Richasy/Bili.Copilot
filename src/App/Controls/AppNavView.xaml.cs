// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 应用导航视图.
/// </summary>
public sealed partial class AppNavView : AppNavViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavView"/> class.
    /// </summary>
    public AppNavView()
    {
        InitializeComponent();
        ViewModel = AppViewModel.Instance;
    }

    /// <summary>
    /// 改变导航布局.
    /// </summary>
    /// <param name="position">位置.</param>
    public void ChangeLayout(string position)
    {
        if (position == "bottom")
        {
            NavContainer.Padding = new Thickness(4, 0, 4, 0);
            NavContainer.Height = 56;
            MainNavView.Height = 48;
            NavContainer.Width = double.NaN;
            MainNavView.Width = double.NaN;
            MainNavView.Margin = new Thickness(0, -4, 0, 0);
            MainNavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
            MainNavView.IsPaneOpen = false;
            SettingIconButton.Visibility = Visibility.Visible;
            SettingFullButton.Visibility = Visibility.Collapsed;
            foreach (var item in ViewModel.NavigateItems)
            {
                if (!item.IsHeader)
                {
                    item.DisplayTitle = default;
                }
                else
                {
                    item.IsVisible = false;
                }
            }
        }
        else
        {
            NavContainer.Padding = new Thickness(0, 0, 0, 0);
            NavContainer.Height = double.NaN;
            MainNavView.Height = double.NaN;
            NavContainer.Width = 240;
            MainNavView.Width = 240;
            MainNavView.Margin = new Thickness(0);
            MainNavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
            MainNavView.IsPaneOpen = true;
            SettingIconButton.Visibility = Visibility.Collapsed;
            SettingFullButton.Visibility = Visibility.Visible;
            foreach (var item in ViewModel.NavigateItems)
            {
                if (!item.IsHeader)
                {
                    item.DisplayTitle = item.Data.Title;
                }
                else
                {
                    item.IsVisible = true;
                }
            }
        }
    }

    private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.Navigate(PageType.Settings);
}

/// <summary>
/// 应用导航视图基类.
/// </summary>
public abstract class AppNavViewBase : ReactiveUserControl<AppViewModel>
{
}
