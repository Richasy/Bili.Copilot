// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 账户模块.
/// </summary>
public sealed partial class AccountModule : AccountModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountModule"/> class.
    /// </summary>
    public AccountModule()
    {
        InitializeComponent();
        ViewModel = AccountViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnSignOutItemClick(object sender, RoutedEventArgs e)
        => TraceLogger.LogSignOut();
}

/// <summary>
/// 账户模块基类.
/// </summary>
public abstract class AccountModuleBase : ReactiveUserControl<AccountViewModel>
{
}
