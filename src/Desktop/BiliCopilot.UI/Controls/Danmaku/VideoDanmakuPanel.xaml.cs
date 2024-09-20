// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Danmaku.Core;
using Microsoft.Extensions.Logging;
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
            ViewModel.ListAdded -= OnDanmakuListAddedAsync;
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
            oldValue.ListAdded -= OnDanmakuListAddedAsync;
            oldValue.RequestClearDanmaku -= OnRequestClearDanmaku;
            oldValue.ProgressChanged -= OnProgressChanged;
            oldValue.PauseDanmaku -= OnPauseDanmaku;
            oldValue.ResumeDanmaku -= OnResumeDanmaku;
            oldValue.RequestRedrawDanmaku -= OnRedrawDanmaku;
            oldValue.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            oldValue.RequestResetStyle -= OnRequestResetStyle;
            oldValue.ExtraSpeedChanged -= OnExtraSpeedChanged;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListAdded += OnDanmakuListAddedAsync;
        newValue.RequestClearDanmaku += OnRequestClearDanmaku;
        newValue.ProgressChanged += OnProgressChanged;
        newValue.PauseDanmaku += OnPauseDanmaku;
        newValue.ResumeDanmaku += OnResumeDanmaku;
        newValue.RequestRedrawDanmaku += OnRedrawDanmaku;
        newValue.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        newValue.RequestResetStyle += OnRequestResetStyle;
        newValue.ExtraSpeedChanged += OnExtraSpeedChanged;
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

    private async void OnDanmakuListAddedAsync(object? sender, IReadOnlyList<DanmakuInformation> e)
    {
        var items = BilibiliDanmakuParser.GetDanmakuList(e, true);
        var isFirstLoad = _cachedDanmakus.Count == 0;
        _cachedDanmakus = _cachedDanmakus.Concat(items).Distinct().ToList();
        if (isFirstLoad)
        {
            await Task.Delay(250);
            Redraw(true);
            return;
        }

        _danmakuController?.AddDanmakuList(items);
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
        ResetSpeed();
    }

    private void ResetSpeed()
    {
        var finalSpeed = ViewModel.DanmakuSpeed * 5 * ViewModel.ExtraSpeed;
        _danmakuController?.SetRollingSpeed(finalSpeed);
    }

    private void Redraw(bool force = false)
    {
        if (!force && (_danmakuController is null || ViewModel is null))
        {
            return;
        }

        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            _danmakuController?.Close();
            _danmakuController = new DanmakuFrostMaster(RootGrid, this.Get<ILogger<DanmakuFrostMaster>>());
            if (_cachedDanmakus.Any())
            {
                _danmakuController.AddDanmakuList(_cachedDanmakus);
            }

            _danmakuController.UpdateTime(Convert.ToUInt32(_lastProgress * 1000));
            ResetDanmakuStyle();
        });
    }
}
