// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.Appearance;

namespace Richasy.BiliKernel.Models.Article;

/// <summary>
/// 文章标识符.
/// </summary>
public readonly struct ArticleIdentifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleIdentifier"/> struct.
    /// </summary>
    /// <param name="id">文章 Id.</param>
    /// <param name="title">文章标题.</param>
    /// <param name="summary">文章总结.</param>
    /// <param name="cover">封面.</param>
    public ArticleIdentifier(string id, string? title, string? summary, BiliImage? cover)
    {
        Id = id;
        Title = title;
        Cover = cover;
        Summary = summary;
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
    /// 视频时长，以秒为单位.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// 视频 Id，属于网站的资源标识符.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// 重写相等运算符.
    /// </summary>
    public static bool operator ==(ArticleIdentifier left, ArticleIdentifier right)
        => left.Equals(right);

    /// <summary>
    /// 重写不等运算符.
    /// </summary>
    public static bool operator !=(ArticleIdentifier left, ArticleIdentifier right)
        => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ArticleIdentifier identifier && Id == identifier.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => $"{Title} | {Id}";
}
