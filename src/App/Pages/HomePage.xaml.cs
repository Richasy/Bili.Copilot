// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Navigation;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 主页.
/// </summary>
public sealed partial class HomePage : HomePageBase
{
    private readonly FixModuleViewModel _fixModuleViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomePage"/> class.
    /// </summary>
    public HomePage()
    {
        InitializeComponent();
        _fixModuleViewModel = FixModuleViewModel.Instance;
        ViewModel = HomePageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string parameter)
        {
            ViewModel.OpenSearchCommand.Execute(parameter);
        }
        else if (e.Parameter is int num)
        {
            if (num == 1)
            {
                ViewModel.OpenFollowsCommand.Execute(default);
            }
            else if (num == 2)
            {
                ViewModel.OpenFansCommand.Execute(default);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = !ViewModel.IsHomeShown;
        CoreViewModel.BackRequest += OnBackRequest;
        CustomModuleTypeSelection.SelectedIndex = ViewModel.CustomModule.GetHashCode();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => CoreViewModel.BackRequest -= OnBackRequest;

    private void OnBackRequest(object sender, EventArgs e)
        => ViewModel.ResetCommand.Execute(default);

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.CustomModule))
        {
            if (ViewModel.CustomModule.GetHashCode() != CustomModuleTypeSelection.SelectedIndex)
            {
                CustomModuleTypeSelection.SelectedIndex = ViewModel.CustomModule.GetHashCode();
            }
        }
    }

    private void OnCustomModuleTypeSegmentedSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        if (ViewModel.CustomModule.GetHashCode() != CustomModuleTypeSelection.SelectedIndex)
        {
            ViewModel.CustomModule = (HomeCustomModuleType)CustomModuleTypeSelection.SelectedIndex;
        }
    }
}

/// <summary>
/// <see cref="HomePage"/> 的基类.
/// </summary>
public abstract class HomePageBase : PageBase<HomePageViewModel>
{
}
