// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Subtitle;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 视频播放器字幕视图模型.
/// </summary>
public sealed partial class SubtitleViewModel : ViewModelBase
{
    private readonly ISubtitleService _service;
    private readonly ILogger<SubtitleViewModel> _logger;

    private string _aid;
    private string _cid;
    private int _position;
    private List<SubtitleInformation> _subtitles;
    private Action? _initializedAction;

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private List<SubtitleMeta>? _metas;

    [ObservableProperty]
    private SubtitleMeta? _selectedMeta;

    [ObservableProperty]
    private string? _currentSubtitle;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleViewModel"/> class.
    /// </summary>
    public SubtitleViewModel(
        ISubtitleService service,
        ILogger<SubtitleViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 重置字幕目标数据.
    /// </summary>
    public void ResetData(string aid, string cid)
    {
        ClearAll();
        _aid = aid;
        _cid = cid;
        IsEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsSubtitleEnabled, true);
    }

    /// <summary>
    /// 设置初始化回调.
    /// </summary>
    public void SetInitializedCallback(Action action)
        => _initializedAction = action;

    /// <summary>
    /// 更新当前播放位置.
    /// </summary>
    public void UpdatePosition(int position)
    {
        _position = position;
        if (SelectedMeta == null || _subtitles is null || _subtitles.Count == 0 || !IsEnabled)
        {
            return;
        }

        var subtitle = _subtitles.FirstOrDefault(p => p.StartPosition <= _position && p.EndPosition >= _position);
        CurrentSubtitle = subtitle?.Content;
    }

    /// <summary>
    /// 清除字幕.
    /// </summary>
    public void ClearSubttile()
        => CurrentSubtitle = default;

    /// <summary>
    /// 清除所有数据.
    /// </summary>
    public void ClearAll()
    {
        _aid = default;
        _cid = default;
        _subtitles = default;
        SelectedMeta = default;
        _position = 0;
        IsAvailable = false;
        CurrentSubtitle = string.Empty;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Metas is not null)
        {
            return;
        }

        try
        {
            Metas = [.. await _service.GetSubtitleMetasAsync(_aid, _cid)];
            IsAvailable = Metas?.Count > 0;
            var firstMeta = GetInitialMeta();
            if (firstMeta is not null)
            {
                ChangeSubtitleCommand.Execute(firstMeta);
            }

            _initializedAction?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载字幕元数据失败");
        }
    }

    [RelayCommand]
    private void DeselectSubtitle()
    {
        SelectedMeta = default;
        CurrentSubtitle = string.Empty;
    }

    [RelayCommand]
    private async Task ChangeSubtitleAsync(SubtitleMeta meta)
    {
        if (meta is null || meta == SelectedMeta)
        {
            return;
        }

        try
        {
            var subtitles = await _service.GetSubtitleDetailAsync(meta);
            if (subtitles is not null)
            {
                _subtitles = [.. subtitles];
                SelectedMeta = meta;
                UpdatePosition(_position);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载字幕失败");
        }
    }

    private SubtitleMeta? GetInitialMeta()
    {
        var firstNonAIMeta = Metas?.FirstOrDefault(p => !p.IsAI);
        if (firstNonAIMeta is not null)
        {
            return firstNonAIMeta;
        }

        return default;
    }

    partial void OnIsEnabledChanged(bool value)
    {
        DeselectSubtitle();
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsSubtitleEnabled, value);
    }
}
