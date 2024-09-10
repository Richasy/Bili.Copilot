// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// AI 快捷项视图模型.
/// </summary>
public sealed partial class AIQuickItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private FluentIcons.Common.Symbol _symbol;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _description;

    /// <summary>
    /// Initializes a new instance of the <see cref="AIQuickItemViewModel"/> class.
    /// </summary>
    public AIQuickItemViewModel(
        FluentIcons.Common.Symbol symbol,
        string title,
        string desc,
        string prompt)
    {
        Symbol = symbol;
        Title = title;
        Description = desc;
        Prompt = prompt;
    }

    /// <summary>
    /// 提示词.
    /// </summary>
    public string Prompt { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AIQuickItemViewModel model && Prompt == model.Prompt;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Prompt);
}
