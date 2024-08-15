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
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartsSection"/> class.
    /// </summary>
    public VideoPartsSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel.SelectedPart is VideoPart part)
        {
            View.Select(ViewModel.Parts.ToList().IndexOf(part));
        }

        InitializeLayoutAsync();
    }

    private void OnPartSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (IsLoaded && sender.SelectedItem is VideoPart part && part != ViewModel.SelectedPart)
        {
            ViewModel.SelectPartCommand.Execute(part);
        }
    }

    private async void OnIndexToggledAsync(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        InitializeLayoutAsync();
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
