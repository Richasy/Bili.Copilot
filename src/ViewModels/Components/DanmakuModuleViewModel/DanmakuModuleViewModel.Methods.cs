// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 弹幕视图模型.
/// </summary>
public sealed partial class DanmakuModuleViewModel
{
    /// <summary>
    /// 转换为弹幕颜色.
    /// </summary>
    /// <param name="hexColor">HEX颜色.</param>
    /// <returns>颜色字符串.</returns>
    private static string ToDanmakuColor(string hexColor)
    {
        var color = CommunityToolkit.WinUI.Helpers.ColorHelper.ToColor(hexColor);
        var num = (color.R * 256 * 256) + (color.G * 256) + (color.B * 1);
        return num.ToString();
    }

    private void Initialize()
    {
        IsShowDanmaku = SettingsToolkit.ReadLocalSetting(SettingNames.IsShowDanmaku, true);
        DanmakuOpacity = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuOpacity, 0.8);
        DanmakuFontSize = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuFontSize, 1.5d);
        DanmakuArea = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuArea, 1d);
        DanmakuSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuSpeed, 1d);
        DanmakuZoom = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuZoom, 1d);
        DanmakuFont = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuFont, "Segoe UI");
        IsDanmakuMerge = SettingsToolkit.ReadLocalSetting(SettingNames.IsDanmakuMerge, false);
        IsDanmakuBold = SettingsToolkit.ReadLocalSetting(SettingNames.IsDanmakuBold, true);
        IsDanmakuLimit = true;
        UseCloudShieldSettings = SettingsToolkit.ReadLocalSetting(SettingNames.UseCloudShieldSettings, true);
        Location = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuLocation, DanmakuLocation.Scroll);

        IsStandardSize = SettingsToolkit.ReadLocalSetting(SettingNames.IsDanmakuStandardSize, true);
        PropertyChanged += OnPropertyChanged;

        TryClear(FontCollection);

        var fontList = FontToolkit.GetFonts();
        fontList.ToList().ForEach(FontCollection.Add);
        LocationCollection.Add(DanmakuLocation.Scroll);
        LocationCollection.Add(DanmakuLocation.Top);
        LocationCollection.Add(DanmakuLocation.Bottom);

        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.White), "#FFFFFF"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Red), "#FE0302"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Orange), "#FFAA02"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Khaki), "#FFD302"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Yellow), "#FFFF00"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Grass), "#A0EE00"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Green), "#00CD00"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Blue), "#019899"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.Purple), "#4266BE"));
        ColorCollection.Add(new KeyValue<string>(ResourceToolkit.GetLocalizedString(StringNames.LightBlue), "#89D5FF"));

        Color = SettingsToolkit.ReadLocalSetting(SettingNames.DanmakuColor, ColorCollection.First().Value);
    }

    [RelayCommand]
    private void Seek(double seconds)
        => _currentSeconds = seconds;

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IsShowDanmaku):
                SettingsToolkit.WriteLocalSetting(SettingNames.IsShowDanmaku, IsShowDanmaku);
                break;
            case nameof(DanmakuOpacity):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuOpacity, DanmakuOpacity);
                break;
            case nameof(DanmakuZoom):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuZoom, DanmakuZoom);
                break;
            case nameof(DanmakuFontSize):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuFontSize, DanmakuFontSize);
                break;
            case nameof(DanmakuArea):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuArea, DanmakuArea);
                break;
            case nameof(DanmakuSpeed):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuSpeed, DanmakuSpeed);
                break;
            case nameof(DanmakuFont):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuFont, DanmakuFont);
                break;
            case nameof(IsDanmakuMerge):
                SettingsToolkit.WriteLocalSetting(SettingNames.IsDanmakuMerge, IsDanmakuMerge);
                break;
            case nameof(IsDanmakuBold):
                SettingsToolkit.WriteLocalSetting(SettingNames.IsDanmakuBold, IsDanmakuBold);
                break;
            case nameof(UseCloudShieldSettings):
                SettingsToolkit.WriteLocalSetting(SettingNames.UseCloudShieldSettings, UseCloudShieldSettings);
                break;
            case nameof(IsStandardSize):
                SettingsToolkit.WriteLocalSetting(SettingNames.IsDanmakuStandardSize, IsStandardSize);
                break;
            case nameof(Location):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuLocation, Location);
                break;
            case nameof(Color):
                SettingsToolkit.WriteLocalSetting(SettingNames.DanmakuColor, Color);
                break;
            default:
                break;
        }
    }
}
