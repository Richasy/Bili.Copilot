// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Models;
using Microsoft.UI.Dispatching;
using Windows.UI;

namespace Bili.Copilot.Libs.Player;

internal static class Utils
{
    public static readonly double SWFREQ_TO_TICKS = 10000000.0 / Stopwatch.Frequency;
    private static int _uniqueId;

    public static List<string> ExtensionsAudio { get; } = new()
    {
        "3ga", "669", "a52", "aac", "ac3",
        "adt", "adts", "aif", "aifc", "aiff",
        "au", "amr", "aob", "ape", "caf",
        "cda", "dts", "flac", "it", "m4a",
        "m4p", "mid", "mka", "mlp", "mod",
        "mp1", "mp2", "mp3", "mpc", "mpga",
        "oga", "oma", "opus", "qcp", "ra",
        "rmi", "snd", "s3m", "spx", "tta",
        "voc", "vqf", "w64", "wav", "wma",
        "wv", "xa", "xm",
    };

    public static List<string> ExtensionsPictures { get; } = new()
    {
        "apng", "bmp", "gif", "jpg", "jpeg", "png", "ico", "tif", "tiff", "tga",
    };

    public static List<string> ExtensionsSubtitles { get; } = new()
    {
        "ass", "ssa", "srt", "sub", "txt", "text", "vtt",
    };

    public static List<string> ExtensionsVideo { get; } = new()
    {
        "3g2", "3gp", "3gp2", "3gpp", "amrec",
        "amv", "asf", "avi", "bik", "divx",
        "drc", "dv", "f4v", "flv", "gvi",
        "gxf", "m1v", "m2t", "m2v", "m2ts",
        "m4v", "mkv", "mov", "mp2v", "mp4",
        "mp4v", "mpa", "mpe", "mpeg", "mpeg1",
        "mpeg2", "mpeg4", "mpg", "mpv2", "mts",
        "mtv", "mxf", "nsv", "nuv", "ogg",
        "ogm", "ogx", "ogv", "rec", "rm",
        "rmvb", "rpl", "thp", "tod", "ts",
        "tts", "vob", "vro", "webm", "wmv",
        "xesc", "dav",
    };

    internal static DispatcherQueue DispatcherQueue { get; set; }

    internal static int GetUniqueId()
    {
        Interlocked.Increment(ref _uniqueId);
        return _uniqueId;
    }

    internal static int GCD(int a, int b) => b == 0 ? a : GCD(b, a % b);

    internal static void UI(Action action) => DispatcherQueue.TryEnqueue(() => { action(); });

    internal static string TicksToTime(long ticks) => new TimeSpan(ticks).ToString();

    internal static void Log(string msg)
    {
        try
        {
            Debug.WriteLine($"[{DateTime.Now:hh.mm.ss.fff}] {msg}");
        }
        catch (Exception)
        {
            Debug.WriteLine($"[............] [MediaFramework] {msg}");
        }
    }

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

    internal static MediaPart GetMediaPart(string title, bool movieOnly = false)
    {
        Match res;
        MediaPart mp = new();
        List<int> indices = new();

        // s|season 01 ... e|episode 01
        res = Regex.Match(title, @"(^|[^a-z0-9])(s|season)[^a-z0-9]*(?<season>[0-9]{1,2})[^a-z0-9]*(e|episode)[^a-z0-9]*(?<episode>[0-9]{1,2})($|[^a-z0-9])", RegexOptions.IgnoreCase);
        if (!res.Success) // 01x01
        {
            res = Regex.Match(title, @"(^|[^a-z0-9])(?<season>[0-9]{1,2})x(?<episode>[0-9]{1,2})($|[^a-z0-9])", RegexOptions.IgnoreCase);
        }

        if (res.Success && res.Groups["season"].Value != string.Empty && res.Groups["episode"].Value != string.Empty)
        {
            mp.Season = int.Parse(res.Groups["season"].Value);
            mp.Episode = int.Parse(res.Groups["episode"].Value);

            if (movieOnly)
            {
                return mp;
            }

            indices.Add(res.Index);
        }

        // non-movie words, 1080p, 2015
        indices.Add(Regex.Match(title, "[^a-z0-9]extended", RegexOptions.IgnoreCase).Index);
        indices.Add(Regex.Match(title, "[^a-z0-9]directors.cut", RegexOptions.IgnoreCase).Index);
        indices.Add(Regex.Match(title, "[^a-z0-9]brrip", RegexOptions.IgnoreCase).Index);
        indices.Add(Regex.Match(title, "[^a-z0-9][0-9]{3,4}p", RegexOptions.IgnoreCase).Index);

        res = Regex.Match(title, @"[^a-z0-9](?<year>(19|20)[0-9][0-9])($|[^a-z0-9])", RegexOptions.IgnoreCase);
        if (res.Success)
        {
            indices.Add(res.Index);
            mp.Year = int.Parse(res.Groups["year"].Value);
        }

        var sorted = indices.OrderBy(x => x);

        foreach (var index in sorted)
        {
            if (index > 0)
            {
                title = title[..index];
                break;
            }
        }

        title = title.Replace(".", " ").Replace("_", " ");
        title = Regex.Replace(title, @"\s{2,}", " ");
        title = Regex.Replace(title, @"[^a-z0-9]$", string.Empty, RegexOptions.IgnoreCase);

        mp.Title = title.Trim();

        return mp;
    }

    internal static string FixFileUrl(string url)
    {
        try
        {
            if (url == null || url.Length < 5)
            {
                return url;
            }

            if (url[..5].ToLower() == "file:")
            {
                return new Uri(url).LocalPath;
            }
        }
        catch
        {
        }

        return url;
    }

    internal static string DownloadToString(string url, int timeoutMs = 30000)
    {
        try
        {
            using HttpClient client = new() { Timeout = TimeSpan.FromMilliseconds(timeoutMs) };
            return client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }
        catch (Exception e)
        {
            Log($"Download failed {e.Message} [Url: {url ?? "Null"}]");
        }

        return null;
    }

    internal static float Scale(float value, float inMin, float inMax, float outMin, float outMax)
        => ((value - inMin) * (outMax - outMin) / (inMax - inMin)) + outMin;

    internal static int Align(int num, int align)
    {
        var mod = num % align;
        return mod == 0 ? num : num + (align - mod);
    }
}
