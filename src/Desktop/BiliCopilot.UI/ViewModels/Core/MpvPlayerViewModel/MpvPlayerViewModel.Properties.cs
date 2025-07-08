// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Models;
using Richasy.MpvKernel.Player;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// Mpv播放器视图模型.
/// </summary>
public sealed partial class MpvPlayerViewModel
{
    private MediaSnapshot _snapshot;
    private IMpvMediaSourceResolver? _sourceResolver;
    private IMpvMediaHistoryResolver? _historyResolver;

    private bool _uiElementSetted;
    private bool _isFirstLoadTrackUpdated = true;
    private double _lastSpeed;
    private double? _continuePosition;

    public event EventHandler<string> WarningOccurred;
    public event EventHandler<double> ProgressChanged;
    public event EventHandler<MpvCacheStateEventArgs> CacheStateChanged;

    public MpvClient? Client { get; private set; }

    public PlayerWindow? Window { get; private set; }

    [ObservableProperty]
    public partial MpvPlayer Player { get; set; }

    [ObservableProperty]
    public partial string? PositionText { get; set; }

    [ObservableProperty]
    public partial string? DurationText { get; set; }

    [ObservableProperty]
    public partial bool IsBlackBackgroundVisible { get; set; } = true;

    [ObservableProperty]
    public partial bool IsControlsVisible { get; set; }

    [ObservableProperty]
    public partial double PreviewPosition { get; set; }

    [ObservableProperty]
    public partial bool IsProgressChanging { get; set; }

    [ObservableProperty]
    public partial bool IsVolumeChanging { get; set; }

    [ObservableProperty]
    public partial bool IsFormatSelectable { get; set; }

    [ObservableProperty]
    public partial bool IsSubtitleEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsAudioSelectable { get; set; }

    [ObservableProperty]
    public partial string? CacheSpeedText { get; set; }

    [ObservableProperty]
    public partial bool IsPrevButtonEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsNextButtonEnabled { get; set; }

    [ObservableProperty]
    public partial string? PrevButtonToolTip { get; set; }

    [ObservableProperty]
    public partial string? NextButtonToolTip { get; set; }

    [ObservableProperty]
    public partial bool IsVideoNavigationAvailable { get; private set; }

    [ObservableProperty]
    public partial bool IsHoldingSpeedChanging { get; set; }

    [ObservableProperty]
    public partial bool IsConnecting { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuLoading { get; set; }

    [ObservableProperty]
    public partial PlayerFormatItemViewModel? SelectedFormat { get; set; }

    [ObservableProperty]
    public partial string? BackwardTip { get; set; }

    [ObservableProperty]
    public partial string? ForwardTip { get; set; }

    [ObservableProperty]
    public partial bool UseIntegrationOperation { get; set; }

    public ObservableCollection<PlayerFormatItemViewModel> Formats { get; } = [];

    public ObservableCollection<SubtitleItemViewModel> Subtitles { get; } = [];

    public ObservableCollection<AudioItemViewModel> Audios { get; } = [];
}
