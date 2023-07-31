// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频项视图模型.
/// </summary>
public sealed partial class VideoItemViewModel
{
    private readonly Action<VideoItemViewModel> _playAction;
    private readonly Action<VideoItemViewModel> _additionalAction;
    private readonly object _additionalData;

    [ObservableProperty]
    private VideoInformation _data;

    [ObservableProperty]
    private UserItemViewModel _publisher;

    [ObservableProperty]
    private string _playCountText;

    [ObservableProperty]
    private string _danmakuCountText;

    [ObservableProperty]
    private string _likeCountText;

    [ObservableProperty]
    private string _durationText;

    [ObservableProperty]
    private bool _isShowScore;

    [ObservableProperty]
    private string _scoreText;

    [ObservableProperty]
    private bool _isShowCommunity;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _canRemove;

    [ObservableProperty]
    private bool _isAISupported;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is VideoItemViewModel model && EqualityComparer<VideoInformation>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
