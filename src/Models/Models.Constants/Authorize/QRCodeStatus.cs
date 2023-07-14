// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Constants.Authorize;

/// <summary>
/// 登录二维码的状态.
/// </summary>
public enum QRCodeStatus
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
