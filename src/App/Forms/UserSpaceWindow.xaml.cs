// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 用户空间窗口.
/// </summary>
public sealed partial class UserSpaceWindow : WindowBase
{
    private readonly UserSpaceViewModel _viewModel = new();
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpaceWindow"/> class.
    /// </summary>
    public UserSpaceWindow(UserProfile user)
    {
        InitializeComponent();
        var title = string.Format(ResourceToolkit.GetLocalizedString(StringNames.UserSpaceTitle), user.Name);
        _viewModel.SetUserProfile(user);
        CustomTitleBar.Title = title;
        Title = title;
        CustomTitleBar.AttachedWindow = this;
        Width = MainWindow.Instance?.Width ?? 500;
        Height = MainWindow.Instance?.Height ?? 700;
        MinWidth = 400;
        MinHeight = 400;
        IsMaximizable = false;
        MainFrame.Navigate(typeof(UserSpacePage), _viewModel);
        Activated += OnActivated;
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        this.CenterOnScreen();
        _isActivated = true;
    }

    private void OnBackButtonClick(object sender, EventArgs e)
    {
        _viewModel.IsInFollows = false;
        _viewModel.IsInFans = false;
    }
}
