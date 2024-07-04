// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.Appearance;

namespace Richasy.BiliKernel.Adapters;

/// <summary>
/// 图片适配器，将视频封面、用户头像等转换为 <see cref="BiliImage"/>.
/// </summary>
public static class ImageAdapter
{
    /// <summary>
    /// 将图片地址转换为 <see cref="BiliImage"/>.
    /// </summary>
    public static BiliImage ConvertToImage(this string uri)
        => new(new(uri));

    /// <summary>
    /// 根据图片地址及大小信息生成缩略图地址（宽高相等）.
    /// </summary>
    public static BiliImage ConvertToImage(this string uri, double size)
        => ConvertToImage(uri, size, size);

    /// <summary>
    /// 根据图片地址及宽高信息生成缩略图地址.
    /// </summary>
    public static BiliImage ConvertToImage(this string uri, double width, double height)
        => new(new(uri), width, height, (w, h) => $"@{w}w_{h}h_1c_100q.jpg");
}
