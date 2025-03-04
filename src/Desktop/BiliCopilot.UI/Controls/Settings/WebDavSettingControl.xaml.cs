// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.WebDav;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// WebDAV 设置控件.
/// </summary>
public sealed partial class WebDavSettingControl : SettingsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavSettingControl"/> class.
    /// </summary>
    public WebDavSettingControl() => InitializeComponent();

    private async void OnEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var config = (sender as FrameworkElement).DataContext as WebDavConfig;
        var dialog = new WebDavConfigDialog(config)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var config = (sender as FrameworkElement).DataContext as WebDavConfig;
        var textTemplate = ResourceToolkit.GetLocalizedString(StringNames.RemoveWebDavConfigWarning);
        var dialog = new ContentDialog
        {
            Title = ResourceToolkit.GetLocalizedString(StringNames.Tip),
            Content = string.Format(textTemplate, config.Name),
            PrimaryButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
            CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Cancel),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.RemoveWebDavConfigCommand.Execute(config);
        }
    }

    private async void OnAddButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new WebDavConfigDialog
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }
}
