// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频章节条目视图模型.
/// </summary>
/// <remarks>
/// 章节数据来自 B 站播放接口的 <c>view_points</c> 字段，点击条目可跳转到对应时间点.
/// </remarks>
public sealed partial class ChapterItemViewModel : ViewModelBase
{
    private readonly Func<ChapterItemViewModel, Task>? _selectAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChapterItemViewModel"/> class.
    /// </summary>
    /// <param name="index">章节序号.</param>
    /// <param name="name">章节标题，为空时按序号生成.</param>
    /// <param name="position">章节起始位置（秒）.</param>
    /// <param name="endPosition">章节结束位置（秒）.</param>
    /// <param name="selectAction">点击章节时的跳转动作.</param>
    public ChapterItemViewModel(int index, string name, double position, double endPosition = 0, Func<ChapterItemViewModel, Task>? selectAction = null)
    {
        Index = index;
        Title = string.IsNullOrEmpty(name)
            ? string.Format(ResourceToolkit.GetLocalizedString(StringNames.ChapterIndexTemplate), index + 1)
            : name;
        Position = position;
        EndPosition = endPosition > position ? endPosition : position;
        _selectAction = selectAction;
    }

    /// <summary>
    /// 章节序号.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 章节标题.
    /// </summary>
    [ObservableProperty]
    public partial string? Title { get; set; }

    /// <summary>
    /// 章节起始位置（秒）.
    /// </summary>
    [ObservableProperty]
    public partial double Position { get; set; }

    /// <summary>
    /// 章节结束位置（秒）.
    /// </summary>
    [ObservableProperty]
    public partial double EndPosition { get; set; }

    /// <summary>
    /// 是否已播放.
    /// </summary>
    [ObservableProperty]
    public partial bool IsPlayed { get; set; }

    /// <summary>
    /// 是否为当前章节.
    /// </summary>
    [ObservableProperty]
    public partial bool IsPlaying { get; set; }

    /// <summary>
    /// 跳转到本章节.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private Task SelectAsync()
        => _selectAction is null ? Task.CompletedTask : _selectAction(this);
}
