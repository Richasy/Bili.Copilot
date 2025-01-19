// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.Danmaku;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Windows.UI;

namespace Danmaku.Core;

/// <summary>
/// BiliBili 弹幕解析器.
/// </summary>
public static class BilibiliDanmakuParser
{
    /// <summary>
    /// 从 protobuf 弹幕列表中获取弹幕.
    /// </summary>
    /// <returns>弹幕条目列表.</returns>
    public static List<DanmakuItem> GetDanmakuList(IEnumerable<DanmakuInformation> danmakuElems, bool mergeDuplicate)
    {
        var list = new List<DanmakuItem>();
        var duplicatedDanmakuDict = new Dictionary<string, List<DuplicatedDanmakuItem>>();

        foreach (var danmakuElem in danmakuElems)
        {
            var mode = DanmakuMode.Unknown;
            if (mergeDuplicate)
            {
                mode = (DanmakuMode)danmakuElem.Mode;
                if ((mode == DanmakuMode.Rolling || mode == DanmakuMode.Top || mode == DanmakuMode.Bottom) && danmakuElem.StartPosition >= 0)
                {
                    var startMs = (uint)(danmakuElem.StartPosition * 1000);

                    if (!duplicatedDanmakuDict.TryGetValue(danmakuElem.Content, out var duplicatedDanmakuList))
                    {
                        duplicatedDanmakuDict.Add(danmakuElem.Content, [new DuplicatedDanmakuItem() { StartMs = startMs, Count = 1 }]);
                    }
                    else
                    {
                        var merged = false;
                        foreach (var duplicatedDanmaku in duplicatedDanmakuList)
                        {
                            if (Math.Abs((int)(startMs - duplicatedDanmaku.StartMs)) <= 20000) // Merge duplicate danmaku in timeframe of 20s
                            {
                                merged = true;
                                duplicatedDanmaku.Count++;
                                break;
                            }
                        }

                        if (merged)
                        {
                            continue;
                        }
                        else
                        {
                            duplicatedDanmakuList.Add(new DuplicatedDanmakuItem() { StartMs = startMs, Count = 1 });
                        }
                    }
                }
            }

            var item = ParseDanmakuItem(danmakuElem);
            if (item != null)
            {
                list.Add(item);
            }
            else
            {
                Debug.WriteLine($"Failed to create danmaku: {danmakuElem.Content}");
            }
        }

        if (duplicatedDanmakuDict.Count > 0)
        {
            foreach (var item in list)
            {
                if (item.Mode == DanmakuMode.Rolling || item.Mode == DanmakuMode.Top || item.Mode == DanmakuMode.Bottom)
                {
                    if (duplicatedDanmakuDict.TryGetValue(item.Text, out var value))
                    {
                        foreach (var duplicatedDanmaku in value)
                        {
                            if (duplicatedDanmaku.Count > 1 && item.StartMs == duplicatedDanmaku.StartMs)
                            {
                                item.Text = $"{item.Text}\u00D7{duplicatedDanmaku.Count}";
                                break;
                            }
                        }
                    }
                }
            }
        }

        BilibiliDanmakuSorter.Sort(list);
        return list;
    }

    /// <summary>
    /// Return null if danmaku can't be parsed.
    /// </summary>
    private static DanmakuItem ParseDanmakuItem(DanmakuInformation danmakuElem)
    {
        try
        {
            var danmakuItem = new DanmakuItem()
            {
                Id = Convert.ToUInt64(danmakuElem.Id),
                HasBorder = false,
                Text = danmakuElem.Content,
                TextColor = ParseColor(danmakuElem.Color),
            };

            var startMs = danmakuElem.StartPosition * 1000;
            if (startMs < 0)
            {
                startMs = 0;
            }

            danmakuItem.StartMs = (uint)startMs;

            var mode = danmakuElem.Mode;
            danmakuItem.Mode = danmakuElem.Mode switch
            {
                1 => DanmakuMode.Rolling,
                4 => DanmakuMode.Bottom,
                5 => DanmakuMode.Top,
                6 => DanmakuMode.ReverseRolling,
                7 => DanmakuMode.Advanced,
                _ => DanmakuMode.Unknown,
            };

            var fontSize = danmakuElem.FontSize;
            switch (danmakuItem.Mode)
            {
                case DanmakuMode.Rolling:
                case DanmakuMode.Bottom:
                case DanmakuMode.Top:
                case DanmakuMode.ReverseRolling:
                    {
                        fontSize -= fontSize % 2 == 1 ? 3 : 2;
                        break;
                    }

                case DanmakuMode.Advanced:
                    {
                        fontSize += 4; // Experimental adjustment
                        break;
                    }
            }

            if (fontSize < 2)
            {
                fontSize = 2;
            }

            danmakuItem.BaseFontSize = fontSize;

            if (danmakuItem.Mode == DanmakuMode.Advanced)
            {
                var content = danmakuElem.Content;
                if (!content.StartsWith("[") || !content.EndsWith("]"))
                {
                    return null;
                }

                danmakuItem.AllowDensityControl = false;

                string[] valueArray;
                try
                {
                    using var document = JsonDocument.Parse(content);
                    var jArray = document.RootElement.EnumerateArray();
                    valueArray = new string[jArray.Count()];
                    for (var i = 0; i < valueArray.Length; i++)
                    {
                        valueArray[i] = jArray.ElementAt(i).GetString();
                    }

                    if (valueArray.Length < 5)
                    {
                        return null;
                    }

                    danmakuItem.Text = WebUtility.HtmlDecode(valueArray[4]).Replace("/n", "\n").Replace("\\n", "\n");
                    if (string.IsNullOrWhiteSpace(danmakuItem.Text))
                    {
                        return null;
                    }

                    danmakuItem.StartX = string.IsNullOrWhiteSpace(valueArray[0]) ? 0f : float.Parse(valueArray[0]);
                    danmakuItem.StartY = string.IsNullOrWhiteSpace(valueArray[1]) ? 0f : float.Parse(valueArray[1]);
                    danmakuItem.EndX = danmakuItem.StartX;
                    danmakuItem.EndY = danmakuItem.StartY;

                    var opacitySplit = valueArray[2].Split('-');
                    danmakuItem.StartAlpha = (byte)(Math.Max(float.Parse(opacitySplit[0]), 0) * byte.MaxValue);
                    danmakuItem.EndAlpha = opacitySplit.Length > 1 ? (byte)(Math.Max(float.Parse(opacitySplit[1]), 0) * byte.MaxValue) : danmakuItem.StartAlpha;

                    danmakuItem.DurationMs = (ulong)(float.Parse(valueArray[3]) * 1000);
                    danmakuItem.TranslationDurationMs = danmakuItem.DurationMs;
                    danmakuItem.TranslationDelayMs = 0;
                    danmakuItem.AlphaDurationMs = danmakuItem.DurationMs;
                    danmakuItem.AlphaDelayMs = 0;

                    if (valueArray.Length >= 7)
                    {
                        danmakuItem.RotateZ = string.IsNullOrWhiteSpace(valueArray[5]) ? 0f : float.Parse(valueArray[5]);
                        danmakuItem.RotateY = string.IsNullOrWhiteSpace(valueArray[6]) ? 0f : float.Parse(valueArray[6]);
                    }
                    else
                    {
                        danmakuItem.RotateZ = 0f;
                        danmakuItem.RotateY = 0f;
                    }

                    if (valueArray.Length >= 11)
                    {
                        danmakuItem.EndX = string.IsNullOrWhiteSpace(valueArray[7]) ? 0f : float.Parse(valueArray[7]);
                        danmakuItem.EndY = string.IsNullOrWhiteSpace(valueArray[8]) ? 0f : float.Parse(valueArray[8]);
                        if (!string.IsNullOrWhiteSpace(valueArray[9]))
                        {
                            danmakuItem.TranslationDurationMs = (ulong)float.Parse(valueArray[9]);
                        }

                        if (!string.IsNullOrWhiteSpace(valueArray[10]))
                        {
                            var translationDelayValue = valueArray[10];
                            danmakuItem.TranslationDelayMs = translationDelayValue == "０"
                                ? 0 : (ulong)float.Parse(translationDelayValue);
                        }
                    }

                    danmakuItem.HasOutline = false;
                    danmakuItem.FontFamilyName = "Consolas"; // Default monospaced font
                    danmakuItem.KeepDefinedFontSize = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to parse advanced mode danmaku: {content} Exception: {ex.Message}");
                    return null;
                }
            }

            return danmakuItem;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to parse danmaku: {danmakuElem.Id} Exception: {ex.Message}");
            return null;
        }
    }

    private static Color ParseColor(uint colorValue)
    {
        colorValue = colorValue & 0xFFFFFF; // Ingore alpha
        var b = 0xFF & colorValue;
        var g = (0xFF00 & colorValue) >> 8;
        var r = (0xFF0000 & colorValue) >> 16;
        return Color.FromArgb(byte.MaxValue, (byte)r, (byte)g, (byte)b);
    }

    private static class BilibiliDanmakuSorter
    {
        public static void Sort(IList<DanmakuItem> list)
            => Merge(list, 0, list.Count - 1);

        private static void Merge(IList<DanmakuItem> list, int p, int r)
        {
            if (p < r)
            {
                var mid = (p + r) / 2;
                Merge(list, p, mid);
                Merge(list, mid + 1, r);
                MergeArray(list, p, mid, r);
            }
        }

        private static void MergeArray(IList<DanmakuItem> list, int p, int mid, int r)
        {
            var tmp = new DanmakuItem[r - p + 1];
            int i = p, j = mid + 1;
            int m = mid, n = r;
            var k = 0;

            while (i <= m && j <= n)
            {
                if (list[i].StartMs < list[j].StartMs)
                {
                    tmp[k++] = list[i++];
                }
                else if (list[i].StartMs > list[j].StartMs)
                {
                    tmp[k++] = list[j++];
                }
                else if (list[i].Mode == DanmakuMode.Advanced)
                {
                    // Compare Id
                    if (list[i].Id <= list[j].Id)
                    {
                        tmp[k++] = list[i++];
                    }
                    else
                    {
                        tmp[k++] = list[j++];
                    }
                }
                else
                {
                    tmp[k++] = list[i++];
                }
            }

            while (i <= m)
            {
                tmp[k++] = list[i++];
            }

            while (j <= n)
            {
                tmp[k++] = list[j++];
            }

            for (i = 0; i < r - p + 1; i++)
            {
                list[p + i] = tmp[i];
            }
        }
    }

    private sealed class DuplicatedDanmakuItem
    {
        public uint StartMs { get; set; }
        public uint Count { get; set; }
    }
}
