// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Data.Appearance;
using Bilibili.App.Dynamic.V2;
using Bilibili.Main.Community.Reply.V1;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 图片适配器，将视频封面、用户头像等转换为 <see cref="Image"/>.
/// </summary>
public static class ImageAdapter
{
    /// <summary>
    /// 将图片地址转换为 <see cref="Image"/>.
    /// </summary>
    /// <param name="uri">图片地址.</param>
    /// <returns><see cref="Image"/>.</returns>
    public static Image ConvertToImage(string uri)
        => new Image(uri);

    /// <summary>
    /// 根据图片地址及宽高信息生成缩略图地址.
    /// </summary>
    /// <param name="uri">图片地址.</param>
    /// <param name="width">图片宽度.</param>
    /// <param name="height">图片高度.</param>
    /// <returns><see cref="Image"/>.</returns>
    public static Image ConvertToImage(string uri, double width, double height)
        => new Image(uri, width, height, (w, h) => $"@{w}w_{h}h_1c_100q.jpg");

    /// <summary>
    /// 根据图片地址生成适用于视频卡片尺寸的缩略图地址.
    /// </summary>
    /// <param name="uri">图片地址.</param>
    /// <returns><see cref="Image"/>.</returns>
    public static Image ConvertToVideoCardCover(string uri)
        => ConvertToImage(uri, AppConstants.VideoCardCoverWidth, AppConstants.VideoCardCoverHeight);

    /// <summary>
    /// 根据图片地址生成适用于 PGC 的竖式封面.
    /// </summary>
    /// <param name="uri">图片地址.</param>
    /// <returns><see cref="Image"/>.</returns>
    public static Image ConvertToPgcCover(string uri)
        => ConvertToImage(uri, AppConstants.PgcCoverWidth, AppConstants.PgcCoverHeight);

    /// <summary>
    /// 根据图片地址生成适用于文章卡片尺寸的缩略图地址.
    /// </summary>
    /// <param name="uri">图片地址.</param>
    /// <returns><see cref="Image"/>.</returns>
    public static Image ConvertToArticleCardCover(string uri)
        => ConvertToImage(uri, AppConstants.ArticleCardCoverWidth, AppConstants.ArticleCardCoverHeight);

    /// <summary>
    /// 将动态的描述模块 <see cref="ModuleDesc"/> 转换为表情文本.
    /// </summary>
    /// <param name="description">动态的描述模块.</param>
    /// <returns><see cref="EmoteText"/>.</returns>
    public static EmoteText ConvertToEmoteText(ModuleDesc description)
    {
        var text = TextToolkit.ConvertToTraditionalChineseIfNeeded(description.Text);
        var descs = description.Desc;
        var emoteDict = new Dictionary<string, Image>();

        // 判断是否有表情存在.
        if (descs.Count > 0 && descs.Any(p => p.Type == DescType.Emoji))
        {
            var emotes = descs.Where(p => p.Type == DescType.Emoji);
            foreach (var item in emotes)
            {
                var t = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Text);
                if (!emoteDict.ContainsKey(t))
                {
                    emoteDict.Add(t, ConvertToImage(item.Uri));
                }
            }
        }
        else
        {
            emoteDict = null;
        }

        return new EmoteText(text, emoteDict);
    }

    /// <summary>
    /// 将评论内容 <see cref="Content"/> 转换为表情文本.
    /// </summary>
    /// <param name="content">评论内容.</param>
    /// <returns><see cref="EmoteText"/>.</returns>
    public static EmoteText ConvertToEmoteText(Content content)
    {
        var text = TextToolkit.ConvertToTraditionalChineseIfNeeded(content.Message);
        var emotes = content.Emote;
        var emoteDict = new Dictionary<string, Image>();

        // 判断是否有表情存在.
        if (emotes?.Count > 0)
        {
            foreach (var item in emotes)
            {
                var k = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Key);
                if (!emoteDict.ContainsKey(k))
                {
                    emoteDict.Add(k, ConvertToImage(item.Value.Url));
                }
            }
        }
        else
        {
            emoteDict = null;
        }

        return new EmoteText(text, emoteDict);
    }
}
