// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Pgc;

/// <summary>
/// 侧边栏时间轴.
/// </summary>
public sealed partial class AnimeTimelineSideControl : AnimeTimelineControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeTimelineSideControl"/> class.
    /// </summary>
    public AnimeTimelineSideControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.TimelineInitialized += OnTimelineInitialized;
        ViewModel.InitializeCommand.Execute(default);
        TimelineView.SelectionChanged += OnTimelineSelectionChanged;
        CheckTimelineSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        TimelineView.SelectionChanged -= OnTimelineSelectionChanged;
        ViewModel.TimelineInitialized -= OnTimelineInitialized;
    }

    private void OnTimelineInitialized(object? sender, EventArgs e)
        => CheckTimelineSelection();

    private void CheckTimelineSelection()
    {
        if (ViewModel.SelectedTimeline is null)
        {
            return;
        }

        var index = ViewModel.Timelines.IndexOf(ViewModel.SelectedTimeline);
        TimelineView.Select(index);
    }

    private void OnTimelineSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is not TimelineItemViewModel vm)
        {
            return;
        }

        ViewModel.SelectTimelineCommand.Execute(vm);
    }
}
