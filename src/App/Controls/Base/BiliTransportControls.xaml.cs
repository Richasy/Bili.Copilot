// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.ComponentModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

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
    public bool IsDanmakuBoxFocused { get; set; }

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
        get => (object)GetValue(DetailContentProperty);
        set => SetValue(DetailContentProperty, value);
    }

    /// <summary>
    /// 将焦点转移到播放按钮上.
    /// </summary>
    public void FocusPlayPauseButton()
    {
        if (ViewModel.DisplayMode == PlayerDisplayMode.CompactOverlay)
        {
            CompactPlayPauseButton.Focus(FocusState.Programmatic);
        }
        else
        {
            PlayPauseButton.Focus(FocusState.Programmatic);
        }
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
            if (ViewModel.IsShowMediaTransport)
            {
                VisualStateManager.GoToState(this, "ControlPanelFadeInState", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "ControlPanelFadeOutState", false);
            }

            FocusPlayPauseButton();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ChangeVisualStateFromStatus();
        ChangeVisualStateFromDisplayMode();
        FocusPlayPauseButton();
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
        if (ViewModel.Status == PlayerStatus.Playing)
        {
            VisualStateManager.GoToState(this, nameof(PlayingState), false);
        }
        else if (ViewModel.Status == PlayerStatus.Pause
            || ViewModel.Status == PlayerStatus.End
            || ViewModel.Status == PlayerStatus.NotLoad)
        {
            VisualStateManager.GoToState(this, nameof(PauseState), false);
        }
        else
        {
            VisualStateManager.GoToState(this, nameof(BufferingState), false);
        }
    }

    private void ChangeVisualStateFromDisplayMode()
    {
        switch (ViewModel.DisplayMode)
        {
            case PlayerDisplayMode.FullScreen:
                VisualStateManager.GoToState(this, nameof(FullScreenState), false);
                break;
            case PlayerDisplayMode.CompactOverlay:
                VisualStateManager.GoToState(this, nameof(CompactState), false);
                break;
            default:
                VisualStateManager.GoToState(this, nameof(NormalState), false);
                break;
        }
    }

    private void OnProgressSliderValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        => ViewModel.ChangeProgressCommand.Execute(e.NewValue);

    private void OnVolumeValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
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

    private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentFormat != null)
        {
            ViewModel.ChangeFormatCommand.Execute(ViewModel.CurrentFormat);
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
