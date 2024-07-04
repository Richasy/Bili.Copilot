// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.Authorization;

namespace Richasy.BiliKernel.Bili.Authorization;

/// <summary>
/// 访问令牌解析器.
/// </summary>
public interface IBiliTokenResolver
{
    /// <summary>
    /// 获取令牌.
    /// </summary>
    /// <returns><see cref="BiliToken"/>.</returns>
    BiliToken? GetToken();

    /// <summary>
    /// 保存令牌.
    /// </summary>
    /// <param name="token">令牌信息.</param>
    void SaveToken(BiliToken token);

    /// <summary>
    /// 移除令牌.
    /// </summary>
    void RemoveToken();
}
