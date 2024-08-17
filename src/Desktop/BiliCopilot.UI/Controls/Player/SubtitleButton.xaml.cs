// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.Subtitle;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 字幕按钮.
/// </summary>
public sealed partial class SubtitleButton : SubtitleControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleButton"/> class.
    /// </summary>
    public SubtitleButton() => InitializeComponent();

    private void OnMetaChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var meta = sender.SelectedItem as SubtitleMeta;
        if (meta != null)
        {
            if (ViewModel.SelectedMeta != meta)
            {
                ViewModel.DeselectSubtitleCommand.Execute(default);
            }
        }
        else
        {
            ViewModel.DeselectSubtitleCommand.Execute(default);
        }
    }

    private void OnFlyoutOpened(object sender, object e)
    {
        if (ViewModel?.Metas is null || ViewModel.SelectedMeta is null)
        {
            SubtitleView.Select(-1);
            return;
        }

        var index = ViewModel.Metas.ToList().IndexOf(ViewModel.SelectedMeta);
        if (index == -1)
        {
            return;
        }

        SubtitleView.Select(index);
    }
}
