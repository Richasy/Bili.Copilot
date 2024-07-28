// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 流行视频排行榜分区视图模型.
/// </summary>
public sealed class PopularRankPartitionViewModel : ViewModelBase<Partition>, IPopularSectionItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularRankPartitionViewModel"/> class.
    /// </summary>
    public PopularRankPartitionViewModel(Partition partition)
        : base(partition)
    {
        Title = partition.Name;
        Logo = partition.Image.Uri;
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// 分区图标.
    /// </summary>
    public Uri Logo { get; init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PopularRankPartitionViewModel model && base.Equals(obj) && EqualityComparer<Partition>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Data);
}
