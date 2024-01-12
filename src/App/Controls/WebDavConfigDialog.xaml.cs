// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// WebDav 配置对话框.
/// </summary>
public sealed partial class WebDavConfigDialog : ContentDialog
{
    private readonly WebDavConfig _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavConfigDialog"/> class.
    /// </summary>
    public WebDavConfigDialog()
    {
        InitializeComponent();
        Title = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.CreateWebDavConfig);
        PortBox.Value = 80;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavConfigDialog"/> class.
    /// </summary>
    public WebDavConfigDialog(WebDavConfig config)
        : this()
    {
        _source = config;
        Title = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.EditWebDavConfig);
        ConfigNameBox.Text = config.Name;
        HostBox.Text = config.Host;
        PortBox.Value = config.Port;
        UserNameBox.Text = config.UserName;
        PasswordBox.Password = config.Password;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var configName = ConfigNameBox.Text;
        var host = HostBox.Text;
        var port = PortBox.Value;
        var userName = UserNameBox.Text;
        var password = PasswordBox.Password;

        if (string.IsNullOrEmpty(configName) || string.IsNullOrEmpty(host)
            || (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password)))
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.InvalidConfig), Models.Constants.App.InfoType.Error);
            return;
        }

        var config = new WebDavConfig
        {
            Id = _source?.Id ?? Guid.NewGuid().ToString(),
            Name = configName,
            Host = host,
            Port = Convert.ToInt32(port),
            UserName = userName,
            Password = password,
        };

        if (_source is null)
        {
            SettingsPageViewModel.Instance.AddWebDavConfigCommand.Execute(config);
        }
        else
        {
            SettingsPageViewModel.Instance.UpdateWebDavConfigCommand.Execute(config);
        }
    }
}
