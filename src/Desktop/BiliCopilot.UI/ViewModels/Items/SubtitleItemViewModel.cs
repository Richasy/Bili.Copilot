// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Subtitle;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

public sealed partial class SubtitleItemViewModel : ViewModelBase<SubtitleMeta>
{
    private readonly Func<SubtitleItemViewModel, Task> _activeFunc;

    public SubtitleItemViewModel(SubtitleMeta meta, Func<SubtitleItemViewModel, Task> activeFunc)
        : base(meta)
    {
        _activeFunc = activeFunc;
        Name = meta.LanguageName;
        IsAI = meta.IsAI;
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial bool IsAI { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public override bool Equals(object? obj) => obj is SubtitleItemViewModel model && Data.Id == model.Data.Id;

    public override int GetHashCode() => HashCode.Combine(Data.Id);

    [RelayCommand]
    private Task ActiveAsync()
        => _activeFunc.Invoke(this);
}
