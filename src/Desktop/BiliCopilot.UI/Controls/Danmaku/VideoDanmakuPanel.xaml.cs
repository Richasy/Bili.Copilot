// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Danmaku.Core;
using Microsoft.Extensions.Logging;
using Microsoft.UI;

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 视频弹幕面板.
/// </summary>
public sealed partial class VideoDanmakuPanel : DanmakuControlBase
{
    private int _lastProgress;
    private DanmakuFrostMaster _danmakuController;

    public VideoDanmakuPanel() => InitializeComponent();

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
            ViewModel.ProgressChanged -= OnProgressChanged;
            ViewModel.PauseDanmaku -= OnPauseDanmaku;
            ViewModel.ResumeDanmaku -= OnResumeDanmaku;
            ViewModel.RequestResetStyle -= OnRequestResetStyle;
            ViewModel.ExtraSpeedChanged -= OnExtraSpeedChanged;
            ViewModel.RequestRedrawDanmaku -= OnRedrawDanmaku;
        }

        _danmakuController?.Close();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DanmakuRenderViewModel? oldValue, DanmakuRenderViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.RequestClearDanmaku -= OnRequestClearDanmaku;
            oldValue.ProgressChanged -= OnProgressChanged;
            oldValue.PauseDanmaku -= OnPauseDanmaku;
            oldValue.ResumeDanmaku -= OnResumeDanmaku;
            oldValue.RequestResetStyle -= OnRequestResetStyle;
            oldValue.ExtraSpeedChanged -= OnExtraSpeedChanged;
            oldValue.RequestRedrawDanmaku -= OnRedrawDanmaku;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.RequestClearDanmaku += OnRequestClearDanmaku;
        newValue.ProgressChanged += OnProgressChanged;
        newValue.PauseDanmaku += OnPauseDanmaku;
        newValue.ResumeDanmaku += OnResumeDanmaku;
        newValue.RequestResetStyle += OnRequestResetStyle;
        newValue.ExtraSpeedChanged += OnExtraSpeedChanged;
        newValue.RequestRedrawDanmaku += OnRedrawDanmaku;
        ResetDanmakuStyle();
    }

    private void OnRequestClearDanmaku(object? sender, EventArgs e)
    {
        if (ViewModel.IsShowDanmaku)
        {
            _lastProgress = 0;
        }

        _danmakuController?.Clear();
    }

    private void OnRequestResetStyle(object? sender, EventArgs e)
        => ResetDanmakuStyle();

    private void OnExtraSpeedChanged(object? sender, EventArgs e)
        => ResetSpeed();

    private void OnProgressChanged(object? sender, int e)
    {
        _lastProgress = e;
        UpdateDanmakuTime(e);
    }

    private void OnResumeDanmaku(object? sender, EventArgs e)
        => _danmakuController?.Resume();

    private void OnPauseDanmaku(object? sender, EventArgs e)
        => _danmakuController?.Pause();

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
        _danmakuController.SetDanmakuFontSizeOffset(ViewModel.DanmakuFontSize);
        _danmakuController.SetFontFamilyName(ViewModel.DanmakuFontFamily);
        _danmakuController.SetIsTextBold(ViewModel.IsDanmakuBold);
        _danmakuController.SetDanmakuEnabledType(ViewModel.IsRollingEnabled, ViewModel.IsTopEnabled, ViewModel.IsBottomEnabled);
        _danmakuController.SetOutlineSize(ViewModel.OutlineSize);
        _danmakuController.SetRefreshRate(ViewModel.DanmakuRefreshRate);
        _danmakuController.ForceSoftwareRenderer(ViewModel.ForceSoftwareRenderer);
        ResetSpeed();
    }

    private void ResetSpeed()
    {
        var finalSpeed = ViewModel.DanmakuSpeed * 5 * ViewModel.ExtraSpeed;
        _danmakuController?.SetRollingSpeed(finalSpeed);
    }

    private void Redraw()
    {
        _danmakuController?.Close();
        _danmakuController = new DanmakuFrostMaster(
            RootGrid,
            SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuRefreshRate, 60),
            SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuForceSoftwareRenderer, false),
            this.Get<ILogger<DanmakuFrostMaster>>());

        if (ViewModel.GetCachedDanmakus().Count > 0)
        {
            _danmakuController.AddDanmakuList(BilibiliDanmakuParser.GetDanmakuList(ViewModel.GetCachedDanmakus(), true));
        }

        UpdateDanmakuTime(_lastProgress);
        ResetDanmakuStyle();
    }

    private void UpdateDanmakuTime(double pos)
    {
        if (pos < 0 || double.IsNaN(pos))
        {
            return;
        }

        _danmakuController?.UpdateTime(Convert.ToUInt32(pos * 1000));
    }
}
