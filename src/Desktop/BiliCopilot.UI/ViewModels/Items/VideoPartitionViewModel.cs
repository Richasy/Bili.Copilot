// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频分区视图模型.
/// </summary>
public sealed partial class VideoPartitionViewModel : ViewModelBase<Partition>
{
    /// <summary>
    /// 子分区.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyCollection<VideoPartitionViewModel>? _children;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopularRankPartitionViewModel"/> class.
    /// </summary>
    public VideoPartitionViewModel(Partition partition)
        : base(partition)
    {
        Title = partition.Name;
        Logo = partition.Image?.Uri;
        if (partition.Children is not null)
        {
            Children = [.. partition.Children.Select(p => new VideoPartitionViewModel(p))];
        }
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// 分区图标.
    /// </summary>
    public Uri? Logo { get; init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VideoPartitionViewModel model && base.Equals(obj) && EqualityComparer<Partition>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Data);
}
