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
    private long _viewModelChangedToken;
    private int _lastProgress;
    private DanmakuViewModel _viewModel;
    private DanmakuFrostMaster _danmakuController;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoDanmakuPanel"/> class.
    /// </summary>
    public VideoDanmakuPanel() => InitializeComponent();

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
        ViewModel.ListAdded += OnDanmakuListAdded;
        ViewModel.RequestClearDanmaku += OnRequestClearDanmaku;
        ViewModel.ProgressChanged += OnProgressChanged;
        ViewModel.PauseDanmaku += OnPauseDanmaku;
        ViewModel.ResumeDanmaku += OnResumeDanmaku;
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
            ViewModel.ListAdded -= OnDanmakuListAdded;
            ViewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            ViewModel.ProgressChanged -= OnProgressChanged;
            ViewModel.PauseDanmaku -= OnPauseDanmaku;
            ViewModel.ResumeDanmaku -= OnResumeDanmaku;
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
            _viewModel.ListAdded -= OnDanmakuListAdded;
            _viewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            _viewModel.ProgressChanged -= OnProgressChanged;
            _viewModel.PauseDanmaku -= OnPauseDanmaku;
            _viewModel.ResumeDanmaku -= OnResumeDanmaku;
            _viewModel.RequestRedrawDanmaku -= OnRedrawDanmaku;
            _viewModel.RequestAddSingleDanmaku -= OnRequestAddSingleDanmaku;
            _viewModel.RequestResetStyle -= OnRequestResetStyle;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ListAdded += OnDanmakuListAdded;
        _viewModel.RequestClearDanmaku += OnRequestClearDanmaku;
        _viewModel.ProgressChanged += OnProgressChanged;
        _viewModel.PauseDanmaku += OnPauseDanmaku;
        _viewModel.ResumeDanmaku += OnResumeDanmaku;
        _viewModel.RequestRedrawDanmaku += OnRedrawDanmaku;
        _viewModel.RequestAddSingleDanmaku += OnRequestAddSingleDanmaku;
        _viewModel.RequestResetStyle += OnRequestResetStyle;
        ResetDanmakuStyle();
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
        _danmakuController.SetFontFamilyName("Segoe UI");
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
            if (_cachedDanmakus.Count > 0)
            {
                _danmakuController.AddDanmakuList(_cachedDanmakus);
                _danmakuController.Resume();
                _danmakuController.Seek(Convert.ToUInt32(_lastProgress * 1000));
            }
        });
    }
}
