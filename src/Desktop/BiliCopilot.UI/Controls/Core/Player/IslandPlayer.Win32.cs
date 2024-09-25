// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 岛播放器.
/// </summary>
public sealed partial class IslandPlayer
{
    private static LRESULT WindowProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        Debug.WriteLine($"[IslandPlayer] Message: {msg}");
        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }
}
