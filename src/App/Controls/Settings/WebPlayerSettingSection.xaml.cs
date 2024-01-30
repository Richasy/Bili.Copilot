// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 网页播放器设置部分.
/// </summary>
public sealed partial class WebPlayerSettingSection : SettingSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebPlayerSettingSection"/> class.
    /// </summary>
    public WebPlayerSettingSection() => InitializeComponent();

    private void OnVerifyButtonClick(object sender, RoutedEventArgs e)
        => AppViewModel.Instance.VerifyWebSignInCommand.Execute(default);
}
