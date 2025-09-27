using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Danmaku;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class DanmakuRenderViewModel : ViewModelBase
{
    public void Initialize(IList<DanmakuInformation> danmakus)
    {
        _cachedDanmakus = danmakus;
        ResetData();
        Redraw();
    }

    public IList<DanmakuInformation> GetCachedDanmakus()
        => _cachedDanmakus is null ? [] : _cachedDanmakus;

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
        ProgressChanged?.Invoke(this, position);
    }

    /// <summary>
    /// 暂停.
    /// </summary>
    public void Pause()
    {
        IsPaused = true;
        PauseDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 恢复.
    /// </summary>
    public void Resume()
    {
        IsPaused = false;
        ResumeDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 重新绘制.
    /// </summary>
    public void Redraw()
    {
        RequestRedrawDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void ClearAll()
    {
        ClearDanmaku();
    }

    /// <summary>
    /// 清理弹幕.
    /// </summary>
    public void ClearDanmaku()
    {
        _cachedDanmakus = default;
        RequestClearDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 重置样式.
    /// </summary>
    public void ResetStyle()
        => RequestResetStyle?.Invoke(this, EventArgs.Empty);

    private void ResetOptions()
    {
        IsShowDanmaku = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsShowDanmaku, true);
        DanmakuOpacity = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuOpacity, 0.8);
        DanmakuFontSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontSize, 1.2d);
        DanmakuArea = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuArea, 1d);
        DanmakuSpeed = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuSpeed, 1.2d);
        DanmakuFontFamily = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        IsDanmakuBold = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuBold, true);
        OutlineSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuOutlineSize, 1d);
        IsRollingEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsRollingDanmakuEnabled, true);
        IsTopEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsTopDanmakuEnabled, true);
        IsBottomEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsBottomDanmakuEnabled, true);
        DanmakuRefreshRate = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuRefreshRate, 60);
        ForceSoftwareRenderer = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuForceSoftwareRenderer, false);
        IsDanmakuLimit = true;
        ResetStyle();
    }

    [RelayCommand]
    private async Task ReloadFontsAsync()
    {
        if (Fonts.Count > 0)
        {
            return;
        }

        var fonts = await this.Get<IFontToolkit>().GetFontsAsync();
        foreach (var item in fonts)
        {
            Fonts.Add(item);
        }

        var localFont = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        if (!Fonts.Contains(localFont))
        {
            localFont = "Segoe UI";
        }

        DanmakuFontFamily = localFont;
    }

    partial void OnIsShowDanmakuChanged(bool value)
    {
        if (value)
        {
            Redraw();
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

    partial void OnIsTopEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsTopDanmakuEnabled, value);

    partial void OnIsBottomEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsBottomDanmakuEnabled, value);

    partial void OnIsRollingEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsRollingDanmakuEnabled, value);

    partial void OnExtraSpeedChanged(double value)
        => ExtraSpeedChanged?.Invoke(this, EventArgs.Empty);

    partial void OnOutlineSizeChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuOutlineSize, value);

    partial void OnDanmakuRefreshRateChanged(int value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuRefreshRate, value);

    partial void OnForceSoftwareRendererChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuForceSoftwareRenderer, value);
}
