// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Authorizers.Tv.Core;

/// <summary>
/// 登录二维码的状态.
/// </summary>
internal enum QRCodeStatus
{
    /// <summary>
    /// 二维码已过期.
    /// </summary>
    Expired,

    /// <summary>
    /// 需要用户确认.
    /// </summary>
    NotConfirm,

    /// <summary>
    /// 已通过验证.
    /// </summary>
    Success,

    /// <summary>
    /// 授权过程中出现错误.
    /// </summary>
    Failed,
}
