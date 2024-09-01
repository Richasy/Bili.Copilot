// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频播放器分P部分.
/// </summary>
public sealed partial class VideoPartsSection : VideoPartsSectionBase
{
    private VideoPlayerPartSectionDetailViewModel _viewModel;
    private long _viewModelChangedToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartsSection"/> class.
    /// </summary>
    public VideoPartsSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.PartChanged += OnPartChanged;
        UpdateSelection();
        InitializeLayoutAsync();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.PartChanged -= OnPartChanged;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.PartChanged -= OnPartChanged;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.PartChanged += OnPartChanged;
        UpdateSelection();
        InitializeLayoutAsync();
    }

    private void OnPartSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (IsLoaded && sender.SelectedItem is VideoPart part && part != ViewModel.SelectedPart)
        {
            ViewModel.SelectPartCommand.Execute(part);
        }
    }

    private void OnPartChanged(object? sender, EventArgs e)
        => UpdateSelection();

    private async void OnIndexToggledAsync(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        InitializeLayoutAsync();
    }

    private void UpdateSelection()
    {
        if (ViewModel?.SelectedPart is VideoPart part)
        {
            View.Select(ViewModel.Parts.ToList().IndexOf(part));
        }
    }

    private async void InitializeLayoutAsync()
    {
        if (ViewModel.OnlyIndex)
        {
            View.Layout = IndexLayout;
            View.ItemTemplate = IndexTemplate;
        }
        else
        {
            View.Layout = DefaultLayout;
            View.ItemTemplate = DefaultTemplate;
        }

        await Task.Delay(100);
        View.StartBringItemIntoView(ViewModel.Parts.ToList().IndexOf(ViewModel.SelectedPart), new BringIntoViewOptions { VerticalAlignmentRatio = 0.5 });
    }
}

/// <summary>
/// 视频播放器分P部分.
/// </summary>
public abstract class VideoPartsSectionBase : LayoutUserControlBase<VideoPlayerPartSectionDetailViewModel>
{
}
