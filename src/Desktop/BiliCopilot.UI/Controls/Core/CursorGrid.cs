// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Input;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 光标网格.
/// </summary>
internal sealed partial class CursorGrid : Grid
{
    /// <summary>
    /// 隐藏光标.
    /// </summary>
    public void HideCursor()
        => ProtectedCursor?.Dispose();

    /// <summary>
    /// 显示光标.
    /// </summary>
    public void ShowCursor()
        => ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
}
