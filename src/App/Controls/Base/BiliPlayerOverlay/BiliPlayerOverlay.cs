// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bili.Copilot.App.Controls.Danmaku;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public sealed partial class BiliPlayerOverlay : ReactiveControl<PlayerDetailViewModel>
{
    private readonly Dictionary<int, List<DanmakuModel>> _danmakuDictionary;
    private DanmakuView _danmakuView;
    private DispatcherTimer _danmakuTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayerOverlay"/> class.
    /// </summary>
    public BiliPlayerOverlay()
    {
        DefaultStyleKey = typeof(BiliPlayerOverlay);
        _danmakuDictionary = new Dictionary<int, List<DanmakuModel>>();
        InitializeDanmakuTimer();
        SizeChanged += OnSizeChanged;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is PlayerDetailViewModel oldViewModel)
        {
            oldViewModel.DanmakuViewModel.DanmakuListAdded -= OnDanmakuListAdded;
            oldViewModel.DanmakuViewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            oldViewModel.DanmakuViewModel.LiveDanmakuAdded -= OnLiveDanmakuAdded;
            oldViewModel.DanmakuViewModel.SendDanmakuSucceeded -= OnSendDanmakuSucceeded;
            oldViewModel.DanmakuViewModel.PropertyChanged -= OnDanmakuViewModelPropertyChanged;
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is PlayerDetailViewModel newViewModel)
        {
            newViewModel.DanmakuViewModel.DanmakuListAdded += OnDanmakuListAdded;
            newViewModel.DanmakuViewModel.RequestClearDanmaku += OnRequestClearDanmaku;
            newViewModel.DanmakuViewModel.LiveDanmakuAdded += OnLiveDanmakuAdded;
            newViewModel.DanmakuViewModel.SendDanmakuSucceeded += OnSendDanmakuSucceeded;
            newViewModel.DanmakuViewModel.PropertyChanged += OnDanmakuViewModelPropertyChanged;
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _danmakuView = GetTemplateChild("DanmakuView") as DanmakuView;

        CheckDanmakuZoom();
    }
}
