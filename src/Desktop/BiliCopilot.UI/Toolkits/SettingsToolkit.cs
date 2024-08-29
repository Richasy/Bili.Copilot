// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Windows.Storage;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// Settings toolkit.
/// </summary>
public static class SettingsToolkit
{
    /// <summary>
    /// Read local setting.
    /// </summary>
    /// <typeparam name="T">Type of read value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="defaultValue">Default value provided when the setting does not exist.</param>
    /// <returns>Setting value obtained.</returns>
    public static T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
        => ReadLocalSetting(settingName.ToString(), defaultValue);

    /// <summary>
    /// Read local setting.
    /// </summary>
    /// <typeparam name="T">Type of read value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="defaultValue">Default value provided when the setting does not exist.</param>
    /// <returns>Setting value obtained.</returns>
    public static T ReadLocalSetting<T>(string settingName, T defaultValue)
    {
        var settingContainer = GetSettingContainer();

        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName].ToString();
                _ = Enum.TryParse(typeof(T), tempValue, out var result);
                return (T)result;
            }
            else
            {
                return (T)settingContainer.Values[settingName];
            }
        }
        else
        {
            WriteLocalSetting(settingName, defaultValue);
            return defaultValue;
        }
    }

    /// <summary>
    /// Write local setting.
    /// </summary>
    /// <typeparam name="T">Type of written value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="value">Setting value.</param>
    public static void WriteLocalSetting<T>(SettingNames settingName, T value)
        => WriteLocalSetting(settingName.ToString(), value);

    /// <summary>
    /// Write local setting.
    /// </summary>
    /// <typeparam name="T">Type of written value.</typeparam>
    /// <param name="settingName">Setting name.</param>
    /// <param name="value">Setting value.</param>
    public static void WriteLocalSetting<T>(string settingName, T value)
    {
        var settingContainer = GetSettingContainer();
        settingContainer.Values[settingName] = value is Enum ? value.ToString() : value;
    }

    /// <summary>
    /// Delete local setting.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    public static void DeleteLocalSetting(SettingNames settingName)
    {
        var settingContainer = GetSettingContainer();

        if (IsSettingKeyExist(settingName))
        {
            _ = settingContainer.Values.Remove(settingName.ToString());
        }
    }

    /// <summary>
    /// Whether the setting to be read has been created locally.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    /// <returns><c>true</c> means the local setting exists, <c>false</c> means it does not exist.</returns>
    public static bool IsSettingKeyExist(SettingNames settingName)
        => IsSettingKeyExist(settingName.ToString());

    /// <summary>
    /// Whether the setting to be read has been created locally.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    /// <returns><c>true</c> means the local setting exists, <c>false</c> means it does not exist.</returns>
    public static bool IsSettingKeyExist(string settingName)
        => GetSettingContainer().Values.ContainsKey(settingName);

    private static ApplicationDataContainer GetSettingContainer()
        => ApplicationData.Current.LocalSettings.CreateContainer("v3", ApplicationDataCreateDisposition.Always);
}
