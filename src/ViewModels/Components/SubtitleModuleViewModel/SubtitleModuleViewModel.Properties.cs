// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 字幕模块视图模型.
/// </summary>
public sealed partial class SubtitleModuleViewModel
{
    private readonly List<SubtitleInformation> _subtitles;

    private string _mainId;
    private string _partId;
    private double _currentSeconds;
    private bool _isLocal;

    [ObservableProperty]
    private string _currentSubtitle;

    [ObservableProperty]
    private SubtitleMeta _currentMeta;

    [ObservableProperty]
    private SubtitleConvertType _convertType;

    [ObservableProperty]
    private bool _hasSubtitles;

    [ObservableProperty]
    private bool _canShowSubtitle;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _canConvert;

    /// <summary>
    /// 元数据变更.
    /// </summary>
    public event EventHandler<SubtitleMeta> MetaChanged;

    /// <summary>
    /// 元数据.
    /// </summary>
    public ObservableCollection<SubtitleMeta> Metas { get; }

    /// <summary>
    /// 转换类型集合.
    /// </summary>
    public ObservableCollection<SubtitleConvertType> ConvertTypeCollection { get; }
}
