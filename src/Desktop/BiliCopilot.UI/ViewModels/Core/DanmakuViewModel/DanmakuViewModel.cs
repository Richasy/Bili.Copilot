// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 弹幕视图模型.
/// </summary>
public sealed partial class DanmakuViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuViewModel"/> class.
    /// </summary>
    public DanmakuViewModel(
        IDanmakuService service,
        ILogger<DanmakuViewModel> logger)
    {
        _danmakuService = service;
        _logger = logger;
    }

    /// <summary>
    /// 重置数据.
    /// </summary>
    public void ResetData(string aid, string cid)
    {
        Clear();
        _aid = aid;
        _cid = cid;
        ResetOptions();
    }

    /// <summary>
    /// 更新进度.
    /// </summary>
    public void UpdateProgress(int progress, int duration)
    {
        _progress = progress;
        _duration = duration;
        var index = Convert.ToInt32(Math.Ceiling(progress / 360d));
        if (index == 0)
        {
            index = 1;
        }

        LoadDanmakusCommand.Execute(index);
        ProgressChanged?.Invoke(this, progress);
    }

    /// <summary>
    /// 暂停.
    /// </summary>
    public void Pause()
        => PauseDanmaku?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 恢复.
    /// </summary>
    public void Resume()
        => ResumeDanmaku?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 重新绘制.
    /// </summary>
    public void Redraw()
        => RequestRedrawDanmaku?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void Clear()
    {
        RequestClearDanmaku?.Invoke(this, EventArgs.Empty);
        _segmentIndex = 0;
        _progress = 0;
        _duration = 0;
        _aid = string.Empty;
        _cid = string.Empty;
    }

    [RelayCommand]
    private async Task LoadDanmakusAsync(int index = 0)
    {
        if (IsLoading || _segmentIndex == index || !IsShowDanmaku)
        {
            return;
        }

        try
        {
            var danmakus = await _danmakuService.GetSegmentDanmakusAsync(_aid, _cid, index);
            if (danmakus.Count > 0)
            {
                ListAdded?.Invoke(this, danmakus);
            }

            _segmentIndex = index;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"加载 {_aid} | {_cid} 的弹幕失败，索引为 {index}");
        }
    }

    private void ResetOptions()
    {
        IsShowDanmaku = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsShowDanmaku, true);
        DanmakuOpacity = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuOpacity, 0.8);
        DanmakuFontSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontSize, 1.5d);
        DanmakuArea = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuArea, 1d);
        DanmakuSpeed = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuSpeed, 1d);
        DanmakuZoom = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuZoom, 1d);
        DanmakuFontFamily = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        IsDanmakuBold = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuBold, false);
        IsDanmakuLimit = true;
        Location = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuLocation, DanmakuLocation.Scroll);

        IsStandardSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuStandardSize, true);
    }
}
