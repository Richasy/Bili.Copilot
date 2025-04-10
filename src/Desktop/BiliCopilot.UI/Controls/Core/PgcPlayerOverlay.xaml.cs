// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using Richasy.MpvKernel.WinUI;
using System.ComponentModel;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// PGC 播放器覆盖层.
/// </summary>
public sealed partial class PgcPlayerOverlay : MpvPlayerControlBase, IMpvUIElement
{
    private readonly DispatcherQueueTimer _controlTimer;
    private Point _lastCursorPoint;
    private bool _isPointerOnUI;

    public PgcSourceViewModel Source { get; }

    public PgcPlayerOverlay(PgcSourceViewModel sourceVM, MpvPlayerWindowViewModel stateVM)
    {
        InitializeComponent();
        _controlTimer = DispatcherQueue.CreateTimer();
        _controlTimer.Interval = TimeSpan.FromMilliseconds(1500);
        _controlTimer.Tick += OnControlTimerTick;
        Source = sourceVM;
        ViewModel = stateVM;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
        => _isPointerOnUI = true;

    protected override void OnPointerExited(PointerRoutedEventArgs e)
        => _isPointerOnUI = false;

    private void OnControlTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_isPointerOnUI || FormatComboBox.IsDropDownOpen || DanmakuBox.IsTextBoxFocused)
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
            Source.IsSeasonSectionPanelVisible = false;
            Source.IsEpisodeSectionPanelVisible = false;

            if (!_controlTimer.IsRunning)
            {
                _controlTimer.Start();
            }
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

    private void OnEpisodeButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsEpisodeSectionPanelLoaded = true;
        Source.IsEpisodeSectionPanelVisible = true;
        Source.EpisodeSection.TryFirstLoadCommand.Execute(default);
        ViewModel.IsControlVisible = false;
    }

    private void OnSeasonButtonClick(object sender, RoutedEventArgs e)
    {
        Source.IsSeasonSectionPanelLoaded = true;
        Source.IsSeasonSectionPanelVisible = true;
        Source.SeasonSection.TryFirstLoadCommand.Execute(default);
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
                && !Source.IsEpisodeSectionPanelVisible
                && !Source.IsSeasonSectionPanelVisible)
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

        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnNextButtonClick(object sender, EventArgs e)
        => Source.PlayNextEpisodeCommand.Execute(default);

    private void OnPrevButtonClick(object sender, EventArgs e)
        => Source.PlayPrevEpisodeCommand.Execute(default);
}
