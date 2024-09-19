// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Danmaku.Core;
using Microsoft.Extensions.Logging;
using Microsoft.UI;

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 直播弹幕面板.
/// </summary>
public sealed partial class LiveDanmakuPanel : DanmakuControlBase
{
    private DanmakuFrostMaster _danmakuController;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveDanmakuPanel"/> class.
    /// </summary>
    public LiveDanmakuPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel is null)
        {
            return;
        }

        ResetDanmakuStyle();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            ViewModel.RequestRedrawDanmaku -= OnRedrawDanmaku;
            ViewModel.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            ViewModel.RequestResetStyle -= OnRequestResetStyle;
        }

        _danmakuController?.Close();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DanmakuViewModel? oldValue, DanmakuViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.RequestClearDanmaku -= OnRequestClearDanmaku;
            oldValue.RequestRedrawDanmaku -= OnRedrawDanmaku;
            oldValue.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            oldValue.RequestResetStyle -= OnRequestResetStyle;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.RequestClearDanmaku += OnRequestClearDanmaku;
        newValue.RequestRedrawDanmaku += OnRedrawDanmaku;
        newValue.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        newValue.RequestResetStyle += OnRequestResetStyle;
        ResetDanmakuStyle();
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

    private void OnRequestClearDanmaku(object? sender, EventArgs e)
    {
        _danmakuController?.Clear();
    }

    private void OnRequestAddSingleDanmaku(object? sender, string e)
    {
        if (_danmakuController is null)
        {
            _danmakuController = new DanmakuFrostMaster(RootGrid, this.Get<ILogger<DanmakuFrostMaster>>());
            ResetDanmakuStyle();
        }

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
    }

    private void Redraw()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _danmakuController?.Close();
            _danmakuController = new DanmakuFrostMaster(RootGrid, this.Get<ILogger<DanmakuFrostMaster>>());
            ResetDanmakuStyle();
        });
    }
}
