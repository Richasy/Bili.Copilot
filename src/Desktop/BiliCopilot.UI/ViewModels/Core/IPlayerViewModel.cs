// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public interface IPlayerViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 设置播放数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task SetPlayDataAsync(string? videoUrl, string? audioUrl, bool isAutoPlay, int position = 0);

    /// <summary>
    /// 注入进度改变时的回调.
    /// </summary>
    void SetProgressAction(Action<int, int> action);

    /// <summary>
    /// 注入状态改变时的回调.
    /// </summary>
    void SetStateAction(Action<PlayerState> action);

    /// <summary>
    /// 注入播放结束时的回调.
    /// </summary>
    void SetEndAction(Action action);

    /// <summary>
    /// 关闭播放器.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task CloseAsync();

    /// <summary>
    /// 显示通知.
    /// </summary>
    void ShowNotification(PlayerNotification notification);

    /// <summary>
    /// 检查底部进度条是否可见.
    /// </summary>
    /// <param name="shouldShow">是否需要显示.</param>
    void CheckBottomProgressVisibility(bool shouldShow);
}
