// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Appearance;

/// <summary>
/// 带表情的文本内容.
/// </summary>
public sealed class EmoteText
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmoteText"/> class.
    /// </summary>
    /// <param name="text">完整文本.</param>
    /// <param name="emotes">表情索引.</param>
    /// <param name="pictures">评论图片.</param>
    public EmoteText(string text, Dictionary<string, BiliImage> emotes, List<BiliImage> pictures = default)
    {
        Text = text;
        Emotes = emotes;
        Pictures = pictures;
    }

    /// <summary>
    /// 完整文本.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// 表情索引.
    /// </summary>
    public Dictionary<string, BiliImage> Emotes { get; }

    /// <summary>
    /// 评论图片.
    /// </summary>
    public List<BiliImage> Pictures { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is EmoteText text && Text == text.Text;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Text);
}
