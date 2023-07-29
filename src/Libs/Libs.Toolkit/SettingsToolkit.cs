// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Constants.App;
using Windows.Storage;

namespace Bili.Copilot.Libs.Toolkit;

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
    {
        var settingContainer = ApplicationData.Current.LocalSettings;

        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName.ToString()].ToString();
                _ = Enum.TryParse(typeof(T), tempValue, out var result);
                return (T)result;
            }
            else
            {
                return (T)settingContainer.Values[settingName.ToString()];
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
    {
        var settingContainer = ApplicationData.Current.LocalSettings;
        settingContainer.Values[settingName.ToString()] = value is Enum ? value.ToString() : value;
    }

    /// <summary>
    /// Delete local setting.
    /// </summary>
    /// <param name="settingName">Setting name.</param>
    public static void DeleteLocalSetting(SettingNames settingName)
    {
        var settingContainer = ApplicationData.Current.LocalSettings;

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
        => ApplicationData.Current.LocalSettings.Values.ContainsKey(settingName.ToString());
    public static void WriteLocalSetting(object shouldAskBeforeWindowClosed, bool v) => throw new NotImplementedException();
}
