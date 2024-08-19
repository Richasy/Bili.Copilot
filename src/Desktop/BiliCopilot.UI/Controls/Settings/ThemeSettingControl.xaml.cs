// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 主题设置控件.
/// </summary>
public sealed partial class ThemeSettingControl : SettingsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSettingControl"/> class.
    /// </summary>
    public ThemeSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ThemePicker.SelectedIndex = ViewModel.AppTheme switch
        {
            ElementTheme.Light => 0,
            ElementTheme.Dark => 1,
            _ => 2,
        };
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var theme = ThemePicker.SelectedIndex switch
        {
            0 => ElementTheme.Light,
            1 => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };

        ViewModel.AppTheme = theme;
    }
}
