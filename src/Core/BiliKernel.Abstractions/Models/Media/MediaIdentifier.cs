// Copyright (c) Richasy. All rights reserved.

using System;
using Richasy.BiliKernel.Models.Appearance;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 媒体标识，表示视频/影视/直播的核心信息.
/// </summary>
public readonly struct MediaIdentifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaIdentifier"/> struct.
    /// </summary>
    /// <param name="id">媒体 Id.</param>
    /// <param name="title">媒体名称.</param>
    /// <param name="cover">封面.</param>
    public MediaIdentifier(string id, string? title, BiliImage? cover)
    {
        Id = id;
        Title = title;
        Cover = cover;
    }

    /// <summary>
    /// 媒体标题.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// 媒体封面.
    /// </summary>
    public BiliImage? Cover { get; }

    /// <summary>
    /// 媒体 Id，属于网站的资源标识符.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(MediaIdentifier left, MediaIdentifier right)
        => left.Equals(right);

    /// <summary>
    /// Not equal.
    /// </summary>
    public static bool operator !=(MediaIdentifier left, MediaIdentifier right)
        => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MediaIdentifier identifier && Id == identifier.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString()
        => $"{Title} | {Id}";
}
