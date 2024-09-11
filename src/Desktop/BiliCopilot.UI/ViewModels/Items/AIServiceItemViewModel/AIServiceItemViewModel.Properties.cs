// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// AI服务项目视图模型.
/// </summary>
public sealed partial class AIServiceItemViewModel
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private ProviderType _providerType;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private ClientConfigBase _config;

    [ObservableProperty]
    private bool _isServerModelVisible;

    [ObservableProperty]
    private bool _isCustomModelsEmpty;

    [ObservableProperty]
    private bool _isSelected;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AIServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);
}
