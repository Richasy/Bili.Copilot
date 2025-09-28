// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models;

public sealed class PlaylistInitializedEventArgs(bool canPrev, bool canNext, string? prevTitle, string? nextTitle) : EventArgs
{
    public bool CanPrev { get; } = canPrev;

    public bool CanNext { get; } = canNext;

    public string? PrevTitle { get; } = prevTitle;

    public string? NextTitle { get; } = nextTitle;
}
