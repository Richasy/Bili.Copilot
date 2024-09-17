// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频分区视图模型.
/// </summary>
public sealed partial class PartitionViewModel : ViewModelBase<Partition>
{
    /// <summary>
    /// 子分区.
    /// </summary>
    [ObservableProperty]
    private List<PartitionViewModel>? _children;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopularRankPartitionViewModel"/> class.
    /// </summary>
    public PartitionViewModel(Partition partition)
        : base(partition)
    {
        Title = partition.Name;
        Logo = partition.Image?.Uri;
        if (partition.Children is not null)
        {
            Children = [.. partition.Children.Select(p => new PartitionViewModel(p))];
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
}
