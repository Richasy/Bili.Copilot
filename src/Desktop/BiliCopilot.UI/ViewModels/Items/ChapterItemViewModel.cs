// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

public sealed partial class ChapterItemViewModel(int index, string name, double position) : ViewModelBase
{
    public int Index { get; set; } = index;

    [ObservableProperty]
    public partial string? Title { get; set; } = string.IsNullOrEmpty(name) ? string.Format(ResourceToolkit.GetLocalizedString(StringNames.ChapterIndexTemplate), index + 1) : name;

    [ObservableProperty]
    public partial double Position { get; set; } = position;

    [ObservableProperty]
    public partial bool IsPlayed { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; set; }
}
