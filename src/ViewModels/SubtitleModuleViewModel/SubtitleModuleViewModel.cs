// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 字幕模块视图模型.
/// </summary>
public sealed partial class SubtitleModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleModuleViewModel"/> class.
    /// </summary>
    public SubtitleModuleViewModel()
    {
        _subtitles = new List<SubtitleInformation>();
        Metas = new ObservableCollection<SubtitleMeta>();
        ConvertTypeCollection = new ObservableCollection<SubtitleConvertType>
        {
            SubtitleConvertType.None,
            SubtitleConvertType.ToTraditionalChinese,
            SubtitleConvertType.ToSimplifiedChinese,
        };

        ConvertType = SettingsToolkit.ReadLocalSetting(SettingNames.SubtitleConvertType, SubtitleConvertType.None);
        CanShowSubtitle = SettingsToolkit.ReadLocalSetting(SettingNames.CanShowSubtitle, true);

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, ReloadCommand);
    }

    /// <summary>
    /// 设置数据.
    /// </summary>
    /// <param name="mainId">主Id.</param>
    /// <param name="partId">分Id.</param>
    public void SetData(string mainId, string partId)
    {
        _mainId = mainId;
        _partId = partId;
        ReloadCommand.ExecuteAsync(default);
    }

    [RelayCommand]
    private void Reset()
    {
        _subtitles.Clear();
        TryClear(Metas);
        CurrentMeta = null;
        HasSubtitles = false;
        CurrentSubtitle = string.Empty;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        Reset();
        var data = await PlayerProvider.GetSubtitleIndexAsync(_mainId, _partId);
        if (data != null
            && data.Count() > 0)
        {
            HasSubtitles = true;
            data.ToList().ForEach(Metas.Add);
            await ChangeMetaAsync(Metas.First());
        }
    }

    [RelayCommand]
    private async Task ChangeMetaAsync(SubtitleMeta meta)
    {
        CurrentMeta = meta;
        _subtitles.Clear();
        CurrentSubtitle = string.Empty;
        var subtitles = await PlayerProvider.GetSubtitleDetailAsync(meta.Url);
        foreach (var subtitle in subtitles)
        {
            _subtitles.Add(subtitle);
        }

        SeekCommand.Execute(_currentSeconds);
    }

    [RelayCommand]
    private void Seek(double sec)
    {
        _currentSeconds = sec;
        if (!HasSubtitles || CurrentMeta == null)
        {
            return;
        }

        var subtitle = _subtitles.FirstOrDefault(p => p.StartPosition <= sec && p.EndPosition >= sec);
        CurrentSubtitle = subtitle != null && !string.IsNullOrEmpty(subtitle.Content)
            ? ConvertType switch
            {
                SubtitleConvertType.ToSimplifiedChinese => ToolGood.Words.WordsHelper.ToSimplifiedChinese(subtitle.Content),
                SubtitleConvertType.ToTraditionalChinese => ToolGood.Words.WordsHelper.ToTraditionalChinese(subtitle.Content),
                _ => subtitle.Content
            }
            : string.Empty;
    }

    partial void OnConvertTypeChanged(SubtitleConvertType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.SubtitleConvertType, value);

    partial void OnCanShowSubtitleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CanShowSubtitle, value);
}
