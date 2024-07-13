// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频信息的基础接口.
/// </summary>
public abstract class VideoBase
{
    /// <summary>
    /// 视频标识符.
    /// </summary>
    public MediaIdentifier Identifier { get; protected set; }

    /// <summary>
    /// 扩展数据.
    /// </summary>
    public Dictionary<string, object>? ExtensionData { get; private set; }

    /// <summary>
    /// 添加扩展数据.
    /// </summary>
    public void AddExtensionIfNotNull<T>(string key, T? data)
    {
        if (data == null)
        {
            return;
        }

        ExtensionData ??= new Dictionary<string, object>();

        if (ExtensionData.ContainsKey(key))
        {
            ExtensionData[key] = data;
            return;
        }

        ExtensionData.Add(key, data);
    }

    /// <summary>
    /// 获取扩展数据.
    /// </summary>
    public T? GetExtensionIfNotNull<T>(string key)
    {
        return ExtensionData?.ContainsKey(key) != true
            ? default
            : (T)ExtensionData[key];
    }
}
