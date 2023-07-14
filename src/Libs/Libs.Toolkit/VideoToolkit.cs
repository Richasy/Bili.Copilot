// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.Libs.Toolkit;

/// <summary>
/// 视频工具.
/// </summary>
public static class VideoToolkit
{
    /// <summary>
    /// 获取视频Id类型.
    /// </summary>
    /// <param name="id">视频Id.</param>
    /// <param name="avId">解析后的 Aid.</param>
    /// <returns><c>av</c> 表示 AV Id, <c>bv</c> 表示 BV Id, 空表示不规范.</returns>
    public static VideoIdType GetVideoIdType(string id, out string avId)
    {
        avId = string.Empty;
        if (id.StartsWith("bv", StringComparison.OrdinalIgnoreCase))
        {
            // 判定为 BV Id.
            return VideoIdType.Bv;
        }
        else
        {
            // 可能是 AV Id.
            id = id.Replace("av", string.Empty, StringComparison.OrdinalIgnoreCase);

            if (long.TryParse(id, out _))
            {
                avId = id;
                return VideoIdType.Av;
            }

            return VideoIdType.Invalid;
        }
    }
}
