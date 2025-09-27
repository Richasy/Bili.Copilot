// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models;

public sealed class PlayerInformationUpdatedEventArgs(string? title, string? subtitle, object? cover) : EventArgs
{
    public string? Title { get; } = title;

    public string? Subtitle { get; } = subtitle;

    public object? Cover { get; } = cover;
}
