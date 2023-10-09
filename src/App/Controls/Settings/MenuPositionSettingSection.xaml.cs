// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 菜单位置设置部分.
/// </summary>
public sealed partial class MenuPositionSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MenuPositionSettingSection"/> class.
    /// </summary>
    public MenuPositionSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => PositionComboBox.SelectedIndex = (int)ViewModel.MenuPosition;

    private void OnPositionSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        var index = PositionComboBox.SelectedIndex;
        if (index == -1)
        {
            return;
        }

        if (ViewModel.MenuPosition == (MenuPosition)index)
        {
            return;
        }

        ViewModel.MenuPosition = (MenuPosition)index;
        TraceLogger.LogMenuPositionChanged(ViewModel.MenuPosition);
    }
}
