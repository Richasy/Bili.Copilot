// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// WebDav 设置部分.
/// </summary>
public sealed partial class WebDavSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavSettingSection"/> class.
    /// </summary>
    public WebDavSettingSection()
    {
        InitializeComponent();
    }

    private async void OnEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var config = (sender as FrameworkElement).DataContext as WebDavConfig;
        var dialog = new WebDavConfigDialog(config);
        dialog.XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot;
        await dialog.ShowAsync();
    }

    private async void OnDeleteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var config = (sender as FrameworkElement).DataContext as WebDavConfig;
        var textTemplate = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.RemoveWebDavConfigWarning);
        var dialog = new TipDialog(string.Format(textTemplate, config.Name));
        dialog.CloseButtonText = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.Cancel);
        dialog.XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            SettingsPageViewModel.Instance.RemoveWebDavConfigCommand.Execute(config);
        }
    }

    private async void OnAddButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new WebDavConfigDialog();
        dialog.XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot;
        await dialog.ShowAsync();
    }
}
