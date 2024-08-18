// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Danmaku.Core;
using Microsoft.UI;

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 直播弹幕面板.
/// </summary>
public sealed partial class LiveDanmakuPanel : DanmakuControlBase
{
    private long _viewModelChangedToken;
    private DanmakuViewModel _viewModel;
    private DanmakuFrostMaster _danmakuController;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveDanmakuPanel"/> class.
    /// </summary>
    public LiveDanmakuPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        _danmakuController ??= new DanmakuFrostMaster(RootGrid, default);

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.RequestClearDanmaku += OnRequestClearDanmaku;
        ViewModel.RequestRedrawDanmaku += OnRedrawDanmaku;
        ViewModel.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        ViewModel.RequestResetStyle += OnRequestResetStyle;
        ResetDanmakuStyle();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        if (ViewModel is not null)
        {
            ViewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            ViewModel.RequestRedrawDanmaku -= OnRedrawDanmaku;
            ViewModel.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            ViewModel.RequestResetStyle -= OnRequestResetStyle;
        }

        _danmakuController?.Close();
        _viewModel = default;
    }

    private static DanmakuFontSize GetFontSize(double fontSize)
    {
        return fontSize switch
        {
            0.5 => DanmakuFontSize.Smallest,
            1 => DanmakuFontSize.Smaller,
            2.0 => DanmakuFontSize.Larger,
            2.5 => DanmakuFontSize.Largest,
            _ => DanmakuFontSize.Normal,
        };
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            _viewModel.RequestRedrawDanmaku -= OnRedrawDanmaku;
            _viewModel.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            _viewModel.RequestResetStyle -= OnRequestResetStyle;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.RequestClearDanmaku += OnRequestClearDanmaku;
        _viewModel.RequestRedrawDanmaku += OnRedrawDanmaku;
        _viewModel.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        _viewModel.RequestResetStyle += OnRequestResetStyle;
        ResetDanmakuStyle();
    }

    private void OnRequestClearDanmaku(object? sender, EventArgs e)
    {
        _danmakuController?.Clear();
    }

    private void OnRequestAddSingleDanmaku(object? sender, string e)
    {
        var model = new DanmakuItem
        {
            StartMs = 0,
            Mode = DanmakuMode.Rolling,
            TextColor = Colors.White,
            BaseFontSize = ViewModel.IsStandardSize ? 20 : 24,
            Text = e,
            HasOutline = true,
        };

        _danmakuController?.AddRealtimeDanmaku(model, false);
    }

    private void OnRequestResetStyle(object? sender, EventArgs e)
        => ResetDanmakuStyle();

    private void OnRedrawDanmaku(object? sender, EventArgs e)
        => Redraw();

    private void ResetDanmakuStyle()
    {
        if (_danmakuController is null || ViewModel is null)
        {
            return;
        }

        if (!ViewModel.IsShowDanmaku)
        {
            _danmakuController.Pause();
        }

        _danmakuController.SetRollingDensity(-1);
        _danmakuController.SetOpacity(ViewModel.DanmakuOpacity);
        _danmakuController.SetBorderColor(Colors.Gray);
        _danmakuController.SetRollingAreaRatio(Convert.ToInt32(ViewModel.DanmakuArea * 10));
        _danmakuController.SetDanmakuFontSizeOffset(GetFontSize(ViewModel.DanmakuFontSize));
        _danmakuController.SetFontFamilyName(ViewModel.DanmakuFontFamily);
        _danmakuController.SetRollingSpeed(Convert.ToInt32(ViewModel.DanmakuSpeed * 5));
        _danmakuController.SetIsTextBold(ViewModel.IsDanmakuBold);
        _danmakuController.SetRenderState(renderDanmaku: true, renderSubtitle: false);
    }

    private void Redraw()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _danmakuController.Close();
            _danmakuController = new DanmakuFrostMaster(RootGrid, default);
            ResetDanmakuStyle();
        });
    }
}
