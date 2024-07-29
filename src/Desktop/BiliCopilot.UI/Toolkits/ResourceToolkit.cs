// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Microsoft.Windows.ApplicationModel.Resources;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public static class ResourceToolkit
{
    private static ResourceLoader _loader;

    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
    {
        _loader ??= new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "Resources");
        return _loader.GetString(stringName.ToString());
    }
}
