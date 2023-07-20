// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Bili.Copilot.Libs.Player.Core;
using Microsoft.UI.Dispatching;
using Windows.UI;

namespace Bili.Copilot.Libs.Player;

internal static class Utils
{
    internal static DispatcherQueue DispatcherQueue { get; set; }

    internal static int GCD(int a, int b) => b == 0 ? a : GCD(b, a % b);

    internal static void UI(Action action) => DispatcherQueue.TryEnqueue(() => { action(); });

    internal static string GetBytesReadable(long i)
    {
        // Determine the suffix and readable value
        string suffix;
        double readable;
        if (i >= 0x1000000000000000) // Exabyte
        {
            suffix = "EB";
            readable = i >> 50;
        }
        else if (i >= 0x4000000000000) // Petabyte
        {
            suffix = "PB";
            readable = i >> 40;
        }
        else if (i >= 0x10000000000) // Terabyte
        {
            suffix = "TB";
            readable = i >> 30;
        }
        else if (i >= 0x40000000) // Gigabyte
        {
            suffix = "GB";
            readable = i >> 20;
        }
        else if (i >= 0x100000) // Megabyte
        {
            suffix = "MB";
            readable = i >> 10;
        }
        else if (i >= 0x400) // Kilobyte
        {
            suffix = "KB";
            readable = i;
        }
        else
        {
            return i.ToString("0 B"); // Byte
        }

        // Divide by 1024 to get fractional value
        readable /= 1024;

        // Return formatted number with suffix
        return readable.ToString("0.## ") + suffix;
    }

    internal static unsafe string BytePtrToStringUtf8(byte* bytePtr)
        => Marshal.PtrToStringUTF8((nint)bytePtr);

    internal static string GetFolderPath(string folder)
    {
        if (folder.StartsWith(":"))
        {
            folder = folder[1..];
            return FindFolderBelow(folder);
        }

        return Path.IsPathRooted(folder) ? folder : Path.GetFullPath(folder);
    }

    internal static string FindFolderBelow(string folder)
    {
        var current = AppDomain.CurrentDomain.BaseDirectory;

        while (current != null)
        {
            if (Directory.Exists(Path.Combine(current, folder)))
            {
                return Path.Combine(current, folder);
            }

            current = Directory.GetParent(current)?.FullName;
        }

        return null;
    }

    internal static string GetRecInnerException(Exception e)
    {
        var dump = string.Empty;
        var cur = e.InnerException;

        for (var i = 0; i < 4; i++)
        {
            if (cur == null)
            {
                break;
            }

            dump += "\r\n - " + cur.Message;
            cur = cur.InnerException;
        }

        return dump;
    }

    internal static Color VorticeToWinUIColor(Vortice.Mathematics.Color sColor)
        => Color.FromArgb(sColor.A, sColor.R, sColor.G, sColor.B);

    internal static Vortice.Mathematics.Color WinUIToVorticeColor(Color wColor)
        => new Vortice.Mathematics.Color(wColor.R, wColor.G, wColor.B, wColor.A);

    internal static List<Language> GetSystemLanguages()
    {
        List<Language> languages = new()
        {
            Language.Chinese,
            Language.English,
        };

        return languages;
    }

    internal static string GetUrlExtension(string url)
        => url.LastIndexOf(".") > 0 ? url[(url.LastIndexOf(".") + 1) ..].ToLower() : string.Empty;

    internal static string FindNextAvailableFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return fileName;
        }

        var tmp = Path.Combine(Path.GetDirectoryName(fileName), Regex.Replace(Path.GetFileNameWithoutExtension(fileName), @"(.*) (\([0-9]+)\)$", "$1"));
        string newName;

        for (var i = 1; i < 101; i++)
        {
            newName = tmp + " (" + i + ")" + Path.GetExtension(fileName);
            if (!File.Exists(newName))
            {
                return newName;
            }
        }

        return null;
    }

    internal static string GetValidFileName(string name)
        => string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
}
