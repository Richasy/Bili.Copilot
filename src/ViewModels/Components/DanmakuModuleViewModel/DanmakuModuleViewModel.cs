// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 弹幕视图模型.
/// </summary>
public sealed partial class DanmakuModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuModuleViewModel"/> class.
    /// </summary>
    public DanmakuModuleViewModel()
    {
        FontCollection = new ObservableCollection<string>();
        LocationCollection = new ObservableCollection<DanmakuLocation>();
        ColorCollection = new ObservableCollection<KeyValue<string>>();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachIsRunningToAsyncCommand(p => IsDanmakuLoading = p, LoadSegmentDanmakuCommand);

        AttachExceptionHandlerToAsyncCommand(
            LogException,
            SendDanmakuCommand,
            ReloadCommand,
            LoadSegmentDanmakuCommand);

        Initialize();
    }

    /// <summary>
    /// 设置数据.
    /// </summary>
    /// <param name="mainId">视频/剧集 Id.</param>
    /// <param name="partId">分部 Id.</param>
    /// <param name="type">视频类型.</param>
    public void SetData(string mainId, string partId, VideoType type)
    {
        _mainId = mainId;
        _partId = partId;
        _videoType = type;

        ReloadCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 重置.
    /// </summary>
    [RelayCommand]
    private void Reset()
    {
        _segmentIndex = 0;
        _currentSeconds = 0;
        RequestClearDanmaku?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsReloading)
        {
            return;
        }

        Reset();
        await LoadSegmentDanmakuAsync(1);
    }

    [RelayCommand]
    private async Task LoadSegmentDanmakuAsync(int index)
    {
        if (IsDanmakuLoading || _segmentIndex == index || !IsShowDanmaku || _videoType == VideoType.Live)
        {
            return;
        }

        var danmakus = await PlayerProvider.GetSegmentDanmakuAsync(_mainId, _partId, index);
        DanmakuListAdded?.Invoke(this, danmakus);
        _segmentIndex = index;
    }

    [RelayCommand]
    private async Task<bool> SendDanmakuAsync(string danmakuText)
    {
        var result = _videoType == VideoType.Live
            ? await SendLiveDanmakuAsync(danmakuText)
            : await SendVideoDanmakuAsync(danmakuText);

        if (!result)
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.FailedToSendDanmaku), InfoType.Error);
        }

        return result;
    }

    /// <summary>
    /// 发送弹幕.
    /// </summary>
    /// <param name="danmakuText">弹幕文本.</param>
    /// <returns>发送结果.</returns>
    [RelayCommand]
    private async Task<bool> SendVideoDanmakuAsync(string danmakuText)
    {
        var result = await PlayerProvider.SendDanmakuAsync(
            danmakuText,
            _mainId,
            _partId,
            Convert.ToInt32(_currentSeconds),
            ToDanmakuColor(Color),
            IsStandardSize,
            Location);

        if (result)
        {
            SendDanmakuSucceeded?.Invoke(this, danmakuText);
        }

        return result;
    }

    /// <summary>
    /// 发送弹幕.
    /// </summary>
    /// <param name="danmakuText">弹幕文本.</param>
    /// <returns>发送结果.</returns>
    [RelayCommand]
    private async Task<bool> SendLiveDanmakuAsync(string danmakuText)
    {
        var result = await LiveProvider.SendDanmakuAsync(
            _mainId,
            danmakuText,
            ToDanmakuColor(Color),
            IsStandardSize,
            Location);

        return result;
    }

    [RelayCommand]
    private void AddLiveDanmaku(LiveDanmakuInformation info)
        => LiveDanmakuAdded?.Invoke(this, info);
}
