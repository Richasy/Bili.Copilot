// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Richasy.MpvKernel.WinUI;
using Richasy.WinUIKernel.Share.Base;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class VideoPlayerOverlay : MpvPlayerControlBase, IMpvUIElement
{
    private readonly DispatcherQueueTimer _controlTimer;
    private Point _lastCursorPoint;
    private bool _isPointerOnUI;

    public VideoSourceViewModel Source { get; }

    public VideoPlayerOverlay(VideoSourceViewModel sourceVM, MpvPlayerWindowViewModel stateVM)
    {
        InitializeComponent();
        _controlTimer = DispatcherQueue.CreateTimer();
        _controlTimer.Interval = TimeSpan.FromMilliseconds(1500);
        _controlTimer.Tick += OnControlTimerTick;
        Source = sourceVM;
        ViewModel = stateVM;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        SizeChanged += OnSizeChanged;
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
        => _isPointerOnUI = true;

    protected override void OnPointerExited(PointerRoutedEventArgs e)
        => _isPointerOnUI = false;

    protected override void OnControlLoaded()
        => SubtitlePresenter.ArrangeSubtitleSize(ActualWidth);

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 0 && e.NewSize.Height > 0 && SubtitlePresenter != null)
        {
            SubtitlePresenter.ArrangeSubtitleSize(e.NewSize.Width);
        }
    }

    private void OnControlTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_isPointerOnUI || FormatComboBox.IsDropDownOpen || SubtitleButton.IsFlyoutOpened || DanmakuBox.IsTextBoxFocused)
        {
            return;
        }

        _controlTimer.Stop();
        ViewModel.Window.HideCursor();
        ViewModel.IsControlVisible = false;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsControlVisible) && ViewModel.IsControlVisible)
        {
            Source.IsInfoSectionPanelVisible = false;
            Source.IsCommentSectionPanelVisible = false;
            Source.IsPlaylistSectionPanelVisible = false;
            Source.IsPartSectionPanelVisible = false;
            Source.IsSeasonSectionPanelVisible = false;
            Source.IsRecommendSectionPanelVisible = false;
            Source.IsAISectionPanelVisible = false;

            if (!_controlTimer.IsRunning)
            {
                _controlTimer.Start();
            }
        }
        else if (e.PropertyName == nameof(ViewModel.IsFileLoading) && !ViewModel.IsFileLoading)
        {
            SubtitlePresenter.ArrangeSubtitleSize(ActualWidth);
        }
    }

    private void OnFormatChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (sender as ComboBox).SelectedItem as PlayerFormatItemViewModel;
        if (item is not null && item != Source.SelectedFormat)
        {
            Source.ChangeFormatCommand.Execute(item);
        }
    }

    private void OnVideoInfoButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsInfoSectionPanelVisible = true;
        ViewModel.IsControlVisible = false;
    }

    private void OnCommentButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsCommentSectionPanelLoaded = true;
        Source.IsCommentSectionPanelVisible = true;
        ViewModel.IsControlVisible = false;
    }

    private void OnPlaylistButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsPlaylistSectionPanelLoaded = true;
        Source.IsPlaylistSectionPanelVisible = true;
        Source.PlaylistSection.TryFirstLoadCommand.Execute(default);
        ViewModel.IsControlVisible = false;
    }

    private void OnPartButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsPartSectionPanelLoaded = true;
        Source.IsPartSectionPanelVisible = true;
        Source.PartSection.TryFirstLoadCommand.Execute(default);
        ViewModel.IsControlVisible = false;
    }

    private void OnSeasonButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsSeasonSectionPanelLoaded = true;
        Source.IsSeasonSectionPanelVisible = true;
        Source.SeasonSection.TryFirstLoadCommand.Execute(default);
        ViewModel.IsControlVisible = false;
    }

    private void OnRecommendButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsRecommendSectionPanelLoaded = true;
        Source.IsRecommendSectionPanelVisible = true;
        Source.RecommendSection.TryFirstLoadCommand.Execute(default);
        ViewModel.IsControlVisible = false;
    }

    private void OnAIButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsAISectionPanelLoaded = true;
        Source.IsAISectionPanelVisible = true;
        Source.AI.InitializeCommand.Execute(default);
        ViewModel.IsControlVisible = false;
    }

    public void HandleUINotify(MpvUIEventId id, object data)
    {
        if (id == MpvUIEventId.PointerMoved)
        {
            var args = data as PointerRoutedEventArgs;
            _lastCursorPoint = args.GetCurrentPoint(this).Position;
            ViewModel.Window.ShowCursor();

            if (!Source.IsInfoSectionPanelVisible
                && !Source.IsCommentSectionPanelVisible
                && !Source.IsPartSectionPanelVisible
                && !Source.IsSeasonSectionPanelVisible
                && !Source.IsAISectionPanelVisible
                && !Source.IsPlaylistSectionPanelVisible
                && !Source.IsRecommendSectionPanelVisible)
            {
                ViewModel.IsControlVisible = true;
                _controlTimer.Stop();
                _controlTimer.Start();
            }
        }
    }

    public void Disconnect()
    {
        if (_controlTimer != null)
        {
            _controlTimer.Stop();
            _controlTimer.Tick -= OnControlTimerTick;
        }

        SizeChanged -= OnSizeChanged;
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnNextButtonClick(object sender, EventArgs e)
        => Source.PlayNextVideoCommand.Execute(default);

    private void OnPrevButtonClick(object sender, EventArgs e)
        => Source.PlayPrevVideoCommand.Execute(default);
}

public abstract class MpvPlayerControlBase : LayoutUserControlBase<MpvPlayerWindowViewModel>;