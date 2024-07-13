// Copyright (c) Richasy. All rights reserved.

using System.Threading.Tasks;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 收藏夹服务.
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// 将视频从收藏夹中移除.
    /// </summary>
    Task RemoveVideoAsync(string favoriteId, string videoId);
}
