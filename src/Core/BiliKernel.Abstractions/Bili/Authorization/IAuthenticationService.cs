// Copyright (c) Richasy. All rights reserved.

using System.Threading.Tasks;

namespace Richasy.BiliKernel.Bili.Authorization;

/// <summary>
/// 登录认证服务.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// 登录.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task SignInAsync();

    /// <summary>
    /// 登出.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task SignOutAsync();

    /// <summary>
    /// 刷新Token.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task RefreshTokenAsync();

    /// <summary>
    /// 确认令牌是否有效。如果无效则抛出异常.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task EnsureTokenAsync();
}
