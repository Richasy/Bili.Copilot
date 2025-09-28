// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

public sealed partial class SourceItemViewModel : ViewModelBase<PlayerFormatInformation>
{
    private readonly Func<SourceItemViewModel, Task> _activeFunc;

    public SourceItemViewModel(PlayerFormatInformation info, Func<SourceItemViewModel, Task> activeFunc)
        : base(info)
    {
        Name = info.Description;
        Id = info.Quality.ToString();
        _activeFunc = activeFunc;
        var userIsVip = this.Get<AccountViewModel>().MyProfile.IsVip ?? false;
        IsEnabled = !info.NeedVip || (info.NeedVip && userIsVip);
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial bool IsEnabled { get; set; }

    public string Id { get; set; }

    public override bool Equals(object? obj) => obj is SourceItemViewModel model && Id == model.Id;
    public override int GetHashCode() => HashCode.Combine(Id);

    [RelayCommand]
    private Task ActiveAsync()
        => _activeFunc.Invoke(this);
}
