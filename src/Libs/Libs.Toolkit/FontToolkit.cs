// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace Bili.Copilot.Libs.Toolkit;

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
    public static IEnumerable<string> GetFonts()
    {
        if (!_fonts.Any())
        {
            var fonts = new InstalledFontCollection();
            _fonts.AddRange(fonts.Families.Where(p => p.IsStyleAvailable(FontStyle.Regular)).Select(f => f.Name));
        }

        return _fonts;
    }
}
