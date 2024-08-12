// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 账户视图模型.
/// </summary>
public sealed partial class AccountViewModel
{
    private readonly IMyProfileService _myProfileService;
    private readonly NavigationViewModel _navService;
    private readonly ILogger<AccountViewModel> _logger;

    [ObservableProperty]
    private UserDetailProfile _myProfile;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private int _momentCount;

    [ObservableProperty]
    private int _followCount;

    [ObservableProperty]
    private int _fansCount;

    [ObservableProperty]
    private bool _hasUnread;

    [ObservableProperty]
    private string _introduce;
}
