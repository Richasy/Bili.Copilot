// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.WebDav;

/// <summary>
/// WebDav 配置对话框.
/// </summary>
public sealed partial class WebDavConfigDialog : ContentDialog
{
    private readonly WebDavConfig? _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavConfigDialog"/> class.
    /// </summary>
    public WebDavConfigDialog()
    {
        InitializeComponent();
        Title = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.CreateWebDavConfig);
        PortBox.Value = 80;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavConfigDialog"/> class.
    /// </summary>
    public WebDavConfigDialog(WebDavConfig config)
    {
        InitializeComponent();
        _source = config;
        Title = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.EditWebDavConfig);
        ConfigNameBox.Text = config.Name;
        HostBox.Text = config.Host;
        PortBox.Value = config.Port ?? 80;
        UserNameBox.Text = config.UserName;
        PasswordBox.Password = config.Password;
        PathBox.Text = config.Path;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var configName = ConfigNameBox.Text;
        var host = HostBox.Text;
        var port = PortBox.Value;
        var userName = UserNameBox.Text;
        var password = PasswordBox.Password;
        var path = PathBox.Text;

        if (string.IsNullOrEmpty(configName) || string.IsNullOrEmpty(host)
            || (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password)))
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.InvalidConfig), InfoType.Error));
            return;
        }

        if (!host.StartsWith("http"))
        {
            host = $"http://{host}";
            if (port == 0)
            {
                port = 80;
            }
        }

        var config = new WebDavConfig
        {
            Id = _source?.Id ?? Guid.NewGuid().ToString(),
            Name = configName,
            Host = host,
            Path = path,
            Port = Convert.ToInt32(port),
            UserName = userName,
            Password = password,
        };

        if (_source is null)
        {
            this.Get<SettingsPageViewModel>().AddWebDavConfigCommand.Execute(config);
        }
        else
        {
            this.Get<SettingsPageViewModel>().UpdateWebDavConfigCommand.Execute(config);
        }
    }

    private void OnHostBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (HostBox.Text.StartsWith("https") && (PortBox.Value == 0 || PortBox.Value == 80))
        {
            PortBox.Value = 443;
        }
        else if (!HostBox.Text.StartsWith("https") && HostBox.Text.StartsWith("http") && (PortBox.Value == 0 || PortBox.Value == 443))
        {
            PortBox.Value = 80;
        }
    }
}
