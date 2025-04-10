// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.Core.Models;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 获取媒体源的解析器.
/// </summary>
public interface IMediaSourceResolver
{
    /// <summary>
    /// 获取媒体源的 ID.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public Task InitializeAsync();

    /// <summary>
    /// 获取播放源.
    /// </summary>
    /// <returns>用于视频播放的源数据.</returns>
    public (string url, MpvPlayOptions options) GetSource();

    /// <summary>
    /// 获取媒体标题.
    /// </summary>
    /// <returns>标题.</returns>
    public string GetTitle();

    /// <summary>
    /// 处理播放器状态变化.
    /// </summary>
    /// <param name="state">新状态.</param>
    public void HandlePlayerStateChanged(MpvPlayerState state);

    /// <summary>
    /// 处理播放进度变化（秒）
    /// </summary>
    public void HandleProgressChanged(double position, double duration);

    /// <summary>
    /// 处理播放速度变化.
    /// </summary>
    public void HandleSpeedChanged(double speed);

    /// <summary>
    /// 因内部的一些操作需要重新加载播放数据时触发.
    /// </summary>
    public event EventHandler RequestReload;

    /// <summary>
    /// 因内部的一些操作需要清除播放数据时触发.
    /// </summary>
    public event EventHandler RequestClear;
}
