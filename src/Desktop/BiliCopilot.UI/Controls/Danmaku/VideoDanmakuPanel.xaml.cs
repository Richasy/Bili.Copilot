// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Richasy.BiliKernel.Models;
using Richasy.Danmaku;
using Richasy.Danmaku.Enums;
using Richasy.Danmaku.Legacy;
using Richasy.Danmaku.Models;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 视频弹幕面板.
/// </summary>
public sealed partial class VideoDanmakuPanel : DanmakuControlBase
{
    private int _lastProgress;
    private DanmakuFrostMaster? _danmakuMaster;
    private DanmakuRenderer? _danmakuRenderer;

    public VideoDanmakuPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel is null)
        {
            return;
        }

        ReloadRenderer();
        ResetDanmakuStyle();
    }

    private void ReloadRenderer()
    {
        var type = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuRenderer, DanmakuRendererType.Win2D);
        if (type == DanmakuRendererType.DirectX && _danmakuRenderer == null)
        {
            _danmakuRenderer = new DanmakuRenderer(RootGrid, this.Get<ILogger<DanmakuRenderer>>());
        }
        else if (type == DanmakuRendererType.Win2D && _danmakuMaster == null)
        {
            _danmakuMaster = new DanmakuFrostMaster(RootGrid);
        }
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
            ViewModel.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _danmakuMaster?.Close();
        _danmakuRenderer?.Dispose();
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
            oldValue.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            oldValue.PropertyChanged -= OnViewModelPropertyChanged;
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
        newValue.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        newValue.PropertyChanged += OnViewModelPropertyChanged;
        ResetDanmakuStyle();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Renderer))
        {
            if (ViewModel.Renderer == DanmakuRendererType.DirectX && _danmakuMaster != null)
            {
                _danmakuMaster.Close();
                _danmakuMaster = null;
                RootGrid.Children.Clear();
                ReloadRenderer();
                ResetDanmakuStyle();
                ViewModel.Redraw();
            }
            else if (ViewModel.Renderer == DanmakuRendererType.Win2D && _danmakuRenderer != null)
            {
                _danmakuRenderer.Dispose();
                _danmakuRenderer = null;
                RootGrid.Children.Clear();
                ReloadRenderer();
                ResetDanmakuStyle();
                ViewModel.Redraw();
            }
        }
    }

    private void OnRequestAddSingleDanmaku(object? sender, string e)
    {
        var location = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuLocation, DanmakuLocation.Scroll);
        var color = AppToolkit.HexToColor(SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuColor, Microsoft.UI.Colors.White.ToString()));
        var isStandardSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuStandardSize, true);
        var model = new DanmakuItem
        {
            StartMs = Convert.ToUInt32(_lastProgress * 1000),
            Mode = (DanmakuMode)((int)location),
            TextColor = color,
            BaseFontSize = isStandardSize ? 20 : 24,
            Text = e,
            HasOutline = true,
            HasBorder = true
        };

        _danmakuMaster?.AddRealtimeDanmaku(model, true);
        _danmakuRenderer?.AddRealtimeDanmaku(model, true);
    }

    private void OnRequestClearDanmaku(object? sender, EventArgs e)
    {
        if (ViewModel.IsShowDanmaku)
        {
            _lastProgress = 0;
        }

        _danmakuMaster?.Clear();
        _danmakuRenderer?.Clear();
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
    {
        _danmakuRenderer?.Resume();
        _danmakuMaster?.Resume();
    }

    private void OnPauseDanmaku(object? sender, EventArgs e)
    {
        _danmakuRenderer?.Pause();
        _danmakuMaster?.Pause();
    }

    private void OnRedrawDanmaku(object? sender, EventArgs e)
        => Redraw();

    private void ResetDanmakuStyle()
    {
        if (ViewModel is null)
        {
            return;
        }

        if (!ViewModel.IsShowDanmaku)
        {
            _danmakuRenderer?.Pause();
            _danmakuMaster?.Pause();
        }

        if (_danmakuMaster != null)
        {
            _danmakuMaster.SetRollingDensity(-1);
            _danmakuMaster.SetOpacity(ViewModel.DanmakuOpacity);
            _danmakuMaster.SetBorderColor(Colors.Gray);
            _danmakuMaster.SetRollingAreaRatio(Convert.ToInt32(ViewModel.DanmakuArea * 10));
            _danmakuMaster.SetDanmakuFontSizeOffset(ViewModel.DanmakuFontSize);
            _danmakuMaster.SetFontFamilyName(ViewModel.DanmakuFontFamily);
            _danmakuMaster.SetIsTextBold(ViewModel.IsDanmakuBold);
            _danmakuMaster.SetDanmakuEnabledType(ViewModel.IsRollingEnabled, ViewModel.IsTopEnabled, ViewModel.IsBottomEnabled);
            _danmakuMaster.SetOutlineSize(ViewModel.OutlineSize);
        }

        if (_danmakuRenderer != null)
        {
            var config = DanmakuConfiguration.CreateDefault();
            config.TextOpacity = ViewModel.DanmakuOpacity;
            config.FontSizeScale = ViewModel.DanmakuFontSize;
            config.RollingAreaRatio = (float)ViewModel.DanmakuArea;
            config.FontFamilyName = ViewModel.DanmakuFontFamily ?? "Segoe UI";
            config.IsTextBold = ViewModel.IsDanmakuBold;
            config.OutlineSize = ViewModel.OutlineSize;

            _danmakuRenderer.SetDanmakuEnabledType(ViewModel.IsRollingEnabled, ViewModel.IsTopEnabled, ViewModel.IsBottomEnabled);
            _danmakuRenderer.UpdateConfiguration(config);
        }

        ResetSpeed();
    }

    private void ResetSpeed()
    {
        var finalSpeed = ViewModel.DanmakuSpeed * 5 * ViewModel.ExtraSpeed;
        _danmakuMaster?.SetRollingSpeed(finalSpeed);
        _danmakuRenderer?.UpdateConfiguration(new DanmakuConfiguration { RollingSpeedBase = (float)finalSpeed });
    }

    private async void Redraw()
    {
        if (_danmakuRenderer == null || _danmakuMaster == null)
        {
            await Task.Delay(500);
        }

        if (ViewModel.GetCachedDanmakus().Count > 0)
        {
            _danmakuMaster?.SetDanmakuList(BilibiliDanmakuParser.GetDanmakuList(ViewModel.GetCachedDanmakus(), true));
            _danmakuRenderer?.SetDanmakuList(BilibiliDanmakuParser.GetDanmakuList(ViewModel.GetCachedDanmakus(), true));
        }

        UpdateDanmakuTime(_lastProgress);
        ResetDanmakuStyle();

        if (!ViewModel.IsPaused)
        {
            _danmakuRenderer?.Resume();
            _danmakuMaster?.Resume();
        }
    }

    private void UpdateDanmakuTime(double pos)
    {
        if (pos < 0 || double.IsNaN(pos))
        {
            return;
        }

        _danmakuRenderer?.UpdateTime(Convert.ToUInt32(pos * 1000));
        _danmakuMaster?.UpdateTime(Convert.ToUInt32(pos * 1000));
    }
}
