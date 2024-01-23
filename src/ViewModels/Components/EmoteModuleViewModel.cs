// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels.Components;

/// <summary>
/// 表情模块视图模型.
/// </summary>
public sealed partial class EmoteModuleViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private EmotePackageViewModel _current;

    [ObservableProperty]
    private bool _isError;

    private EmoteModuleViewModel()
    {
        Packages = new ObservableCollection<EmotePackageViewModel>();

        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(
            ex =>
            {
                LogException(ex);
                IsError = true;
            },
            InitializeCommand);
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static EmoteModuleViewModel Instance { get; } = new EmoteModuleViewModel();

    /// <summary>
    /// 表情包集合.
    /// </summary>
    public ObservableCollection<EmotePackageViewModel> Packages { get; }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Packages.Count > 0)
        {
            return;
        }

        IsError = false;
        var packages = await CommunityProvider.GetEmotePackagesAsync();
        foreach (var item in packages)
        {
            Packages.Add(new EmotePackageViewModel(item));
        }

        SelectPackageCommand.Execute(Packages.First());
    }

    [RelayCommand]
    private void SelectPackage(EmotePackageViewModel vm)
    {
        foreach (var item in Packages)
        {
            item.IsSelected = vm.Equals(item);
        }

        Current = vm;
    }
}
