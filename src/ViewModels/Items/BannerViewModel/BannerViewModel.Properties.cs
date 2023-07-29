// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 横幅视图模型.
/// </summary>
public sealed partial class BannerViewModel
{
    [ObservableProperty]
    private string _uri;

    [ObservableProperty]
    private string _cover;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private bool _isTooltipEnabled;

    [ObservableProperty]
    private double _minHeight;

    [ObservableProperty]
    private BannerIdentifier _data;
}
