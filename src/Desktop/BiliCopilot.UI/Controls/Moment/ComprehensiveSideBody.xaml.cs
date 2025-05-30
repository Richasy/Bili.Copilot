// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 综合动态分区侧边栏.
/// </summary>
public sealed partial class ComprehensiveSideBody : ComprehensiveSectionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveSideBody"/> class.
    /// </summary>
    public ComprehensiveSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.Initialized += OnInitialized;
        }

        SectionView.SelectionChanged += OnSectionSelectionChanged;
        CheckSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        SectionView.ItemsSource = default;
        if (ViewModel is not null)
        {
            ViewModel.Initialized -= OnInitialized;
        }

        SectionView.SelectionChanged -= OnSectionSelectionChanged;
    }

    private void OnInitialized(object? sender, EventArgs e)
        => CheckSelection();

    private void OnSectionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as MomentUperSectionViewModel;
        ViewModel?.SelectUperCommand.Execute(item);
    }

    private void CheckSelection()
    {
        if (ViewModel?.SelectedUper is not null)
        {
            SectionView.Select(ViewModel.Upers.IndexOf(ViewModel.SelectedUper));
        }
    }
}
