// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 流行视频区块项视图模型.
/// </summary>
public sealed class PopularSectionItemViewModel : ViewModelBase, IPopularSectionItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularSectionItemViewModel"/> class.
    /// </summary>
    public PopularSectionItemViewModel(
        FluentIcons.Common.Symbol symbol,
        string title,
        PopularSectionType type)
    {
        Icon = symbol;
        Title = title;
        Type = type;
    }

    /// <summary>
    /// 图标.
    /// </summary>
    public FluentIcons.Common.Symbol Icon { get; init; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// 类型.
    /// </summary>
    public PopularSectionType Type { get; init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PopularSectionItemViewModel model && Type == model.Type;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type);
}
