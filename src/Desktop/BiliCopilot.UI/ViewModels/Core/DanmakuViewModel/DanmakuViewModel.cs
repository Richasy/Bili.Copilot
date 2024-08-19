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
        Locations = [DanmakuLocation.Scroll, DanmakuLocation.Top, DanmakuLocation.Bottom];
        Colors = [Microsoft.UI.Colors.White, Microsoft.UI.Colors.Red, Microsoft.UI.Colors.Orange, Microsoft.UI.Colors.Khaki, Microsoft.UI.Colors.Yellow, Microsoft.UI.Colors.GreenYellow, Microsoft.UI.Colors.Green, Microsoft.UI.Colors.Blue, Microsoft.UI.Colors.Purple, Microsoft.UI.Colors.LightBlue];
    }

    /// <summary>
    /// 重置数据.
    /// </summary>
    public void ResetData(string aid, string cid)
    {
        ClearAll();
        _aid = aid;
        _cid = cid;
        _segmentIndex = -1;
        ResetData();
    }

    /// <summary>
    /// 重置数据.
    /// </summary>
    public void ResetData()
    {
        ReloadFontsCommand.Execute(default);
        ResetOptions();
    }

    /// <summary>
    /// 更新进度.
    /// </summary>
    public void UpdatePosition(int position)
    {
        _position = position;
        var index = Convert.ToInt32(Math.Ceiling(position / 360d));
        if (index == 0)
        {
            index = 1;
        }

        LoadDanmakusCommand.Execute(index);
        ProgressChanged?.Invoke(this, position);
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
    public void ClearAll()
    {
        _segmentIndex = -1;
        _position = 0;
        _aid = string.Empty;
        _cid = string.Empty;
        ClearDanmaku();
    }

    /// <summary>
    /// 清理弹幕.
    /// </summary>
    public void ClearDanmaku()
        => RequestClearDanmaku?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 重置样式.
    /// </summary>
    public void ResetStyle()
        => RequestResetStyle?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private async Task LoadDanmakusAsync(int index = 0)
    {
        if (IsLoading || _segmentIndex == index || !IsShowDanmaku)
        {
            return;
        }

        try
        {
            IsLoading = true;
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
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SendDanmakuAsync(string text)
    {
        try
        {
            var danmakuColor = (Color.R * 256 * 256) + (Color.G * 256) + Color.B;
            await _danmakuService.SendDanmakuAsync(text, _aid, _cid, _position, danmakuColor.ToString(), IsStandardSize, Location);
            RequestAddSingleDanmaku?.Invoke(this, text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "弹幕发送失败.");
        }
    }

    [RelayCommand]
    private void AddDanmaku(string text)
    {
        RequestAddSingleDanmaku?.Invoke(this, text);
    }

    private void ResetOptions()
    {
        IsShowDanmaku = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsShowDanmaku, true);
        DanmakuOpacity = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuOpacity, 0.8);
        DanmakuFontSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontSize, 1.5d);
        DanmakuArea = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuArea, 1d);
        DanmakuSpeed = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuSpeed, 1.2d);
        DanmakuFontFamily = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        IsDanmakuBold = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuBold, true);
        IsDanmakuLimit = true;
        Location = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuLocation, DanmakuLocation.Scroll);
        Color = AppToolkit.HexToColor(SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuColor, Microsoft.UI.Colors.White.ToString()));
        IsStandardSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuStandardSize, true);
        ResetStyle();
    }

    [RelayCommand]
    private async Task ReloadFontsAsync()
    {
        var fonts = await FontToolkit.GetFontsAsync();
        Fonts = fonts.ToList();
        var localFont = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        if (!Fonts.Contains(localFont))
        {
            DanmakuFontFamily = "Segoe UI";
        }
    }

    partial void OnIsShowDanmakuChanged(bool value)
    {
        if (value)
        {
            Redraw();
        }
        else
        {
            ResetStyle();
        }

        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsShowDanmaku, value);
    }

    partial void OnDanmakuOpacityChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuOpacity, value);

    partial void OnDanmakuFontSizeChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuFontSize, value);

    partial void OnIsDanmakuBoldChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsDanmakuBold, value);

    partial void OnDanmakuAreaChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuArea, value);

    partial void OnDanmakuSpeedChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuSpeed, value);

    partial void OnDanmakuFontFamilyChanged(string value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, value);

    partial void OnIsStandardSizeChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsDanmakuStandardSize, value);

    partial void OnLocationChanged(DanmakuLocation value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuLocation, value);

    partial void OnColorChanged(Windows.UI.Color value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuColor, value.ToString());
}
