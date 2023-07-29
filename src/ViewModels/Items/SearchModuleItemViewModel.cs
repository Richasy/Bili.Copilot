// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索模块条目视图模型.
/// </summary>
public sealed partial class SearchModuleItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private SearchModuleType _type;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchModuleItemViewModel"/> class.
    /// </summary>
    public SearchModuleItemViewModel(SearchModuleType type, string title, bool isEnabled = true)
    {
        Type = type;
        Title = title;
        IsEnabled = isEnabled;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SearchModuleItemViewModel model && Type == model.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => Type.GetHashCode();
}
