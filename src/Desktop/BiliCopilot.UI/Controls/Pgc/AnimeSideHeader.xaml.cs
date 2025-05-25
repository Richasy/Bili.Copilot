// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Pgc;

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
    protected override void OnControlLoaded()
    {
        ViewModel.SectionInitialized += OnSectionInitialized;
        SectionSelector.SelectionChanged += OnSectionSelectionChanged;
        CheckSectionInitialization();
        CheckSectionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SectionInitialized -= OnSectionInitialized;
        SectionSelector.SelectionChanged -= OnSectionSelectionChanged;
    }

    private void OnSectionInitialized(object? sender, EventArgs e)
    {
        CheckSectionInitialization();
        CheckSectionSelection();
    }

    private void OnSectionSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem?.Tag is not IPgcSectionDetailViewModel vm)
        {
            return;
        }

        ViewModel.SelectSectionCommand.Execute(vm);
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.SelectedSection is null)
        {
            return;
        }

        SectionSelector.SelectedItem = SectionSelector.Items.FirstOrDefault(p => p.Tag == ViewModel.SelectedSection);
    }

    private void CheckSectionInitialization()
    {
        if (ViewModel?.Sections is null || SectionSelector is null || SectionSelector.Items.Count > 0)
        {
            return;
        }

        foreach (var vm in ViewModel.Sections)
        {
            var item = SectionTemplate.LoadContent() as SelectorBarItem;
            item.DataContext = vm;
            SectionSelector.Items.Add(item);
        }
    }
}
