// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 互动视频模块视图模型.
/// </summary>
public sealed partial class InteractionModuleViewModel
{
    private string _partId;
    private string _choiceId;
    private string _graphVersion;

    [ObservableProperty]
    private bool _isReloading;

    /// <summary>
    /// 没有更多选项时触发.
    /// </summary>
    public event EventHandler NoMoreChoices;

    /// <summary>
    /// 选项集合.
    /// </summary>
    public ObservableCollection<InteractionInformation> Choices { get; }
}
