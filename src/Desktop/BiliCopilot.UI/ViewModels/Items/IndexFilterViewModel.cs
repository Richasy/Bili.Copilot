// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 索引过滤器视图模型.
/// </summary>
public sealed partial class IndexFilterViewModel : ViewModelBase<Filter>
{
    private readonly Action? _reloadAction;

    [ObservableProperty]
    private List<Condition>? _conditions;

    [ObservableProperty]
    private Condition? _currentCondition;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexFilterViewModel"/> class.
    /// </summary>
    public IndexFilterViewModel(Filter filter, Action? reloadAction)
        : base(filter)
    {
        Conditions = [.. filter.Conditions];
        CurrentCondition = Conditions.FirstOrDefault();
        _reloadAction = reloadAction;
    }

    [RelayCommand]
    private void ChangeCondition(Condition condition)
    {
        if (condition is null || condition == CurrentCondition)
        {
            return;
        }

        CurrentCondition = condition;
        _reloadAction?.Invoke();
    }
}
