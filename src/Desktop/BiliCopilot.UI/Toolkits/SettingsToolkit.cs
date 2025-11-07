// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// Settings toolkit.
/// </summary>
internal sealed class SettingsToolkit : SharedSettingsToolkit
{
    private Microsoft.Windows.Storage.ApplicationDataContainer _settingContainer;

    /// <summary>
    /// Read local setting.
    /// </summary>
    /// <typeparam name="T">Type of read value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="defaultValue">Default value provided when the setting does not exist.</param>
    /// <returns>Setting value obtained.</returns>
    public static T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().ReadLocalSetting(settingName.ToString(), defaultValue);

    /// <summary>
    /// Write local setting.
    /// </summary>
    /// <typeparam name="T">Type of written value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="value">Setting value.</param>
    public static void WriteLocalSetting<T>(SettingNames settingName, T value)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().WriteLocalSetting(settingName.ToString(), value);

    /// <summary>
    /// Delete local setting.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    public static void DeleteLocalSetting(SettingNames settingName)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().DeleteLocalSetting(settingName.ToString());

    /// <summary>
    /// Whether the setting to be read has been created locally.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    /// <returns><c>true</c> means the local setting exists, <c>false</c> means it does not exist.</returns>
    public static bool IsSettingKeyExist(SettingNames settingName)
        => GlobalDependencies.Kernel.GetRequiredService<ISettingsToolkit>().IsSettingKeyExist(settingName.ToString());

    protected override Microsoft.Windows.Storage.ApplicationDataContainer GetSettingContainer()
    {
        if (_settingContainer != null)
        {
            return _settingContainer;
        }

        _settingContainer = Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalSettings.CreateContainer("v1", Microsoft.Windows.Storage.ApplicationDataCreateDisposition.Always);
        return _settingContainer;
    }
}