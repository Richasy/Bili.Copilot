// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩媒体传输控件.
/// </summary>
public sealed partial class BiliTransportControls : BiliTransportControlsBase
{
    /// <summary>
    /// <see cref="IsLive"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsLiveProperty =
        DependencyProperty.Register(nameof(IsLive), typeof(bool), typeof(BiliTransportControls), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="DetailContent"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DetailContentProperty =
        DependencyProperty.Register(nameof(DetailContent), typeof(object), typeof(BiliTransportControls), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliTransportControls"/> class.
    /// </summary>
    public BiliTransportControls()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 当详情按钮被点击时发生.
    /// </summary>
    public event EventHandler DetailButtonClicked;

    /// <summary>
    /// 光标是否停留在控件上.
    /// </summary>
    public bool IsPointerStay { get; set; }

    /// <summary>
    /// 弹幕输入框是否聚焦.
    /// </summary>
    public bool IsDanmakuBoxFocused => DanmakuBox.IsInputFocused;

    /// <summary>
    /// 是否为直播控件.
    /// </summary>
    public bool IsLive
    {
        get => (bool)GetValue(IsLiveProperty);
        set => SetValue(IsLiveProperty, value);
    }

    /// <summary>
    /// 详情信息.
    /// </summary>
    public object DetailContent
    {
        get => GetValue(DetailContentProperty);
        set => SetValue(DetailContentProperty, value);
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is PlayerDetailViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChanged;
        }

        var vm = e.NewValue as PlayerDetailViewModel;
        if (vm == null)
        {
            return;
        }

        vm.PropertyChanged -= OnViewModelPropertyChanged;
        vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
        => IsPointerStay = true;

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
        => IsPointerStay = true;

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerRoutedEventArgs e)
        => IsPointerStay = false;

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Status))
        {
            ChangeVisualStateFromStatus();
        }
        else if (e.PropertyName == nameof(ViewModel.DisplayMode))
        {
            ChangeVisualStateFromDisplayMode();
        }
        else if (e.PropertyName == nameof(ViewModel.IsShowMediaTransport))
        {
            _ = ViewModel.IsShowMediaTransport
                ? VisualStateManager.GoToState(this, "ControlPanelFadeInState", false)
                : VisualStateManager.GoToState(this, "ControlPanelFadeOutState", false);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ChangeVisualStateFromStatus();
        ChangeVisualStateFromDisplayMode();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel == null)
        {
            return;
        }

        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void ChangeVisualStateFromStatus()
    {
        _ = ViewModel.Status == PlayerStatus.Playing
            ? VisualStateManager.GoToState(this, nameof(PlayingState), false)
            : ViewModel.Status is PlayerStatus.Pause
                        or PlayerStatus.End
                        or PlayerStatus.NotLoad
                ? VisualStateManager.GoToState(this, nameof(PauseState), false)
                : VisualStateManager.GoToState(this, nameof(BufferingState), false);
    }

    private void ChangeVisualStateFromDisplayMode()
    {
        _ = ViewModel.DisplayMode switch
        {
            PlayerDisplayMode.FullScreen => VisualStateManager.GoToState(this, nameof(FullScreenState), false),
            PlayerDisplayMode.CompactOverlay => VisualStateManager.GoToState(this, nameof(CompactState), false),
            _ => VisualStateManager.GoToState(this, nameof(NormalState), false),
        };
    }

    private void OnProgressSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        => ViewModel.ChangeProgressCommand.Execute(e.NewValue);

    private void OnVolumeValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            ViewModel.ChangeVolumeCommand.Execute(Convert.ToInt32(e.NewValue));
        }
    }

    private void OnFormatSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        if (FormatListView.SelectedItem is FormatInformation info
                && ViewModel.CurrentFormat != info)
        {
            ViewModel.ChangeFormatCommand.Execute(info);
        }
    }

    private void OnDetailButtonClick(object sender, RoutedEventArgs e)
        => DetailButtonClicked?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// <see cref="BiliTransportControls"/> 的基类.
/// </summary>
public abstract class BiliTransportControlsBase : ReactiveUserControl<PlayerDetailViewModel>
{
}
