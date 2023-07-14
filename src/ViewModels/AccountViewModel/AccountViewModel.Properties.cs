// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.User;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 账户视图模型.
/// </summary>
public sealed partial class AccountViewModel
{
    private AccountInformation _accountInformation;
    private bool _isInitialized = false;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _introduce;

    [ObservableProperty]
    private string _levelImage;

    [ObservableProperty]
    private string _avatar;

    [ObservableProperty]
    private string _dynamicCount;

    [ObservableProperty]
    private string _followCount;

    [ObservableProperty]
    private string _fansCount;

    [ObservableProperty]
    private string _coinCount;

    [ObservableProperty]
    private string _messageCount;

    [ObservableProperty]
    private bool _isInitializing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AccountViewModel Instance { get; } = new();
}
