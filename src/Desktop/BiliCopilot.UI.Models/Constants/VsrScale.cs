// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models.Constants;

/// <summary>
/// 视频超分辨率枚举.
/// </summary>
public enum VsrScale
{
    None,
#pragma warning disable CA1707 // 标识符不应包含下划线
    x1_5,
    x2_0,
    x3_0,
    x4_0,
#pragma warning restore CA1707 // 标识符不应包含下划线
}
