// Copyright (c) Bili Copilot. All rights reserved.

using System.Drawing;
using System.Drawing.Text;

namespace BiliCopilot.UI.Toolkits;

/// <summary>
/// 字体工具类.
/// </summary>
public static class FontToolkit
{
    private static readonly List<string> _fonts = new();

    /// <summary>
    /// 获取系统字体列表.
    /// </summary>
    /// <returns>字体列表.</returns>
    public static async Task<IList<string>> GetFontsAsync()
    {
        if (_fonts.Count == 0)
        {
            await Task.Run(() =>
            {
                var fonts = new InstalledFontCollection();
                _fonts.AddRange(fonts.Families.Where(p => p.IsStyleAvailable(FontStyle.Regular)).Select(f => f.Name));
            });
        }

        return _fonts;
    }
}
