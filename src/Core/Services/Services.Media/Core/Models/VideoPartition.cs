// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

/// <summary>
/// 分区类型.
/// </summary>
internal sealed class VideoPartition
{
    /// <summary>
    /// 分区的标识符.
    /// </summary>
    [JsonPropertyName("tid")]
    public int Tid { get; set; }

    /// <summary>
    /// 分区名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 分区图标.
    /// </summary>
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    /// <summary>
    /// 分区指向的链接.
    /// </summary>
    /// <remarks>
    /// 该链接指向的是移动端的跳转链接，这里仅用作分析，不支持跳转.
    /// </remarks>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    /// <summary>
    /// 分区子项.
    /// </summary>
    [JsonPropertyName("children")]
    public IList<VideoPartition>? Children { get; set; }

    /// <summary>
    /// 是否是动漫分区.
    /// </summary>
    [JsonPropertyName("is_bangumi")]
    public int? IsBangumi { get; set; }

    /// <summary>
    /// 判断该分区是否需要显示.
    /// </summary>
    /// <remarks>
    /// 部分分区是以H5页面的形式呈现，部分分区是广告，此处仅显示以视频为主的常规分区.
    /// </remarks>
    /// <returns>分区是否需要显示.</returns>
    public bool IsNeedToShow()
    {
        var needToShow = !string.IsNullOrEmpty(Uri) &&
            Uri.StartsWith("bilibili") &&
            Uri.Contains("region/") &&
            Children != null &&
            Children.Count > 0;
        return needToShow;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is VideoPartition partition && Tid == partition.Tid;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Tid);
}
