// Copyright (c) Richasy. All rights reserved.

using System;
using Richasy.BiliKernel.Models.Appearance;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频标识，表示视频的核心信息.
/// </summary>
public readonly struct VideoIdentifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoIdentifier"/> struct.
    /// </summary>
    /// <param name="id">视频 Id.</param>
    /// <param name="title">视频名称.</param>
    /// <param name="cover">封面.</param>
    public VideoIdentifier(string id, string? title, BiliImage? cover)
    {
        Id = id;
        Title = title;
        Cover = cover;
    }

    /// <summary>
    /// 视频标题.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// 视频封面.
    /// </summary>
    public BiliImage? Cover { get; }

    /// <summary>
    /// 视频 Id，属于网站的资源标识符.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(VideoIdentifier left, VideoIdentifier right)
        => left.Equals(right);

    /// <summary>
    /// Not equal.
    /// </summary>
    public static bool operator !=(VideoIdentifier left, VideoIdentifier right)
        => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is VideoIdentifier identifier && Id == identifier.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString()
        => $"{Title} | {Id}";
}
