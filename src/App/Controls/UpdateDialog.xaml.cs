// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 应用更新对话框.
/// </summary>
public sealed partial class UpdateDialog : ContentDialog
{
    private readonly UpdateEventArgs _eventArgs;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDialog"/> class.
    /// </summary>
    public UpdateDialog(UpdateEventArgs args)
    {
        InitializeComponent();
        _eventArgs = args;
        Initialize();
    }

    private void Initialize()
    {
        TitleBlock.Text = _eventArgs.ReleaseTitle;
        PreReleaseContainer.Visibility = _eventArgs.IsPreRelease ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        PublishTimeBlock.Text = _eventArgs.PublishTime.ToString("yyyy/MM/dd HH:mm");
        MarkdownBlock.Text = _eventArgs.ReleaseDescription;
    }

    private async void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        => await Launcher.LaunchUriAsync(_eventArgs.DownloadUrl);

    private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IgnoreVersion, _eventArgs.Version);
}
