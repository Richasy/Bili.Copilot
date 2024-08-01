// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Anime;

/// <summary>
/// 侧边栏头部.
/// </summary>
public sealed partial class AnimeSideHeader : AnimePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeSideHeader"/> class.
    /// </summary>
    public AnimeSideHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.SectionInitialized += OnSectionInitialized;
        SectionSelector.SelectionChanged += OnSectionSelectionChanged;
        CheckSectionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SectionInitialized -= OnSectionInitialized;
        SectionSelector.SelectionChanged -= OnSectionSelectionChanged;
    }

    private void OnSectionInitialized(object? sender, EventArgs e) => CheckSectionSelection();

    private void OnSectionSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SectionSelector.SelectedItem is null || ViewModel.Sections is null)
        {
            return;
        }

        var index = SectionSelector.SelectedIndex;
        var section = ViewModel.Sections.First(p => p.SectionType == (AnimeSectionType)index);
        ViewModel.SelectSectionCommand.Execute(section);
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.SelectedSection is null)
        {
            return;
        }

        SectionSelector.SelectedIndex = (int)ViewModel.SelectedSection.SectionType;
    }
}
