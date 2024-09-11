// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Toolkits;

namespace BiliCopilot.UI.Controls.Settings;

/// <summary>
/// 豆包聊天配置设置部分.
/// </summary>
public sealed partial class DouBaoChatConfigSettingSection : AIServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DouBaoChatConfigSettingSection"/> class.
    /// </summary>
    public DouBaoChatConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Logo.Provider = ViewModel.ProviderType.ToString();
        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), ViewModel.Name);
        KeyBox.PlaceholderText = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyPlaceholder), ViewModel.Name);

        ViewModel.Config ??= new DouBaoClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
