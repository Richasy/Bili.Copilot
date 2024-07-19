// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Windows.ApplicationModel.Resources.Core;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public static class ResourceToolkit
{
    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
        => ResourceManager.Current.MainResourceMap.GetValueOrDefault($"Resources/{stringName}")?.Candidates?.FirstOrDefault()?.ValueAsString ?? string.Empty;
}
