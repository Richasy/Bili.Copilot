// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Popular;

/// <summary>
/// 流行视频页面侧边栏主体.
/// </summary>
public sealed partial class PopularSideBody : PopularPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularSideBody"/> class.
    /// </summary>
    public PopularSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.SectionInitialized += OnSectionInitialized;
        SectionView.SelectionChanged += OnSectionSelectionChanged;
        CheckSectionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SectionInitialized -= OnSectionInitialized;
        SectionView.SelectionChanged -= OnSectionSelectionChanged;
    }

    private void OnSectionInitialized(object? sender, EventArgs e)
        => CheckSectionSelection();

    private void OnSectionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as IPopularSectionItemViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.SelectedSection is not null)
        {
            SectionView.Select(ViewModel.Sections.IndexOf(ViewModel.SelectedSection));
        }
    }
}
