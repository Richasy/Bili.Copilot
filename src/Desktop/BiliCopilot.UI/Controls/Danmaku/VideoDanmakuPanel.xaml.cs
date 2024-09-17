// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Danmaku.Core;
using Microsoft.UI;
using Richasy.BiliKernel.Models.Danmaku;

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 视频弹幕面板.
/// </summary>
public sealed partial class VideoDanmakuPanel : DanmakuControlBase
{
    private List<DanmakuItem> _cachedDanmakus = new();
    private int _lastProgress;
    private DanmakuFrostMaster _danmakuController;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoDanmakuPanel"/> class.
    /// </summary>
    public VideoDanmakuPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        _danmakuController ??= new DanmakuFrostMaster(RootGrid, default);

        if (ViewModel is null)
        {
            return;
        }

        ViewModel.ListAdded += OnDanmakuListAdded;
        ViewModel.RequestClearDanmaku += OnRequestClearDanmaku;
        ViewModel.ProgressChanged += OnProgressChanged;
        ViewModel.PauseDanmaku += OnPauseDanmaku;
        ViewModel.ResumeDanmaku += OnResumeDanmaku;
        ViewModel.RequestRedrawDanmaku += OnRedrawDanmaku;
        ViewModel.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        ViewModel.RequestResetStyle += OnRequestResetStyle;
        ViewModel.ExtraSpeedChanged += OnExtraSpeedChanged;
        ResetDanmakuStyle();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListAdded -= OnDanmakuListAdded;
            ViewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            ViewModel.ProgressChanged -= OnProgressChanged;
            ViewModel.PauseDanmaku -= OnPauseDanmaku;
            ViewModel.ResumeDanmaku -= OnResumeDanmaku;
            ViewModel.RequestRedrawDanmaku -= OnRedrawDanmaku;
            ViewModel.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            ViewModel.RequestResetStyle -= OnRequestResetStyle;
            ViewModel.ExtraSpeedChanged -= OnExtraSpeedChanged;
        }

        _danmakuController?.Close();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DanmakuViewModel? oldValue, DanmakuViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListAdded -= OnDanmakuListAdded;
            oldValue.RequestClearDanmaku -= OnRequestClearDanmaku;
            oldValue.ProgressChanged -= OnProgressChanged;
            oldValue.PauseDanmaku -= OnPauseDanmaku;
            oldValue.ResumeDanmaku -= OnResumeDanmaku;
            oldValue.RequestRedrawDanmaku -= OnRedrawDanmaku;
            oldValue.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            oldValue.RequestResetStyle -= OnRequestResetStyle;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListAdded += OnDanmakuListAdded;
        newValue.RequestClearDanmaku += OnRequestClearDanmaku;
        newValue.ProgressChanged += OnProgressChanged;
        newValue.PauseDanmaku += OnPauseDanmaku;
        newValue.ResumeDanmaku += OnResumeDanmaku;
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

    private void OnDanmakuListAdded(object? sender, IReadOnlyList<DanmakuInformation> e)
    {
        var items = BilibiliDanmakuXmlParser.GetDanmakuList(e, true);
        _danmakuController?.AddDanmakuList(items);
        _cachedDanmakus = _cachedDanmakus.Concat(items).Distinct().ToList();
    }

    private void OnRequestClearDanmaku(object? sender, EventArgs e)
    {
        _cachedDanmakus.Clear();
        _danmakuController?.Clear();
        _lastProgress = 0;
    }

    private void OnRequestAddSingleDanmaku(object? sender, string e)
    {
        var model = new DanmakuItem
        {
            StartMs = Convert.ToUInt32(_lastProgress * 1000),
            Mode = (DanmakuMode)((int)ViewModel.Location),
            TextColor = ViewModel.Color,
            BaseFontSize = ViewModel.IsStandardSize ? 20 : 24,
            Text = e,
            HasOutline = true,
        };

        _danmakuController?.AddRealtimeDanmaku(model, true);
    }

    private void OnRequestResetStyle(object? sender, EventArgs e)
        => ResetDanmakuStyle();

    private void OnExtraSpeedChanged(object? sender, EventArgs e)
        => ResetSpeed();

    private void OnProgressChanged(object? sender, int e)
    {
        _lastProgress = e;
        _danmakuController?.UpdateTime(Convert.ToUInt32(e * 1000));
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
        _danmakuController.SetDanmakuFontSizeOffset(GetFontSize(ViewModel.DanmakuFontSize));
        _danmakuController.SetFontFamilyName(ViewModel.DanmakuFontFamily);
        _danmakuController.SetIsTextBold(ViewModel.IsDanmakuBold);
        _danmakuController.SetRenderState(renderDanmaku: true, renderSubtitle: false);
        ResetSpeed();
    }

    private void ResetSpeed()
    {
        var finalSpeed = ViewModel.DanmakuSpeed * 5 * ViewModel.ExtraSpeed;
        _danmakuController.SetRollingSpeed(finalSpeed);
    }

    private void Redraw()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _danmakuController.Close();
            _danmakuController = new DanmakuFrostMaster(RootGrid, default);
            if (_cachedDanmakus.Any())
            {
                _danmakuController.AddDanmakuList(_cachedDanmakus);
            }

            _danmakuController.UpdateTime(Convert.ToUInt32(_lastProgress * 1000));
            ResetDanmakuStyle();
        });
    }
}
