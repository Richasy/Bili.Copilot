﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml.Navigation;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 主页.
/// </summary>
public sealed partial class HomePage : HomePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HomePage"/> class.
    /// </summary>
    public HomePage()
    {
        InitializeComponent();
        ViewModel = HomePageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string parameter)
        {
            ViewModel.OpenSearchCommand.Execute(parameter);
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = !ViewModel.IsHomeShown;
        CoreViewModel.BackRequest += OnBackRequest;
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => CoreViewModel.BackRequest -= OnBackRequest;

    private void OnBackRequest(object sender, EventArgs e)
    {
        ViewModel.IsInSearch = false;
        ViewModel.IsInMessage = false;
        ViewModel.IsInFans = false;
        ViewModel.IsInFollows = false;
    }
}

/// <summary>
/// <see cref="HomePage"/> 的基类.
/// </summary>
public abstract class HomePageBase : PageBase<HomePageViewModel>
{
}
