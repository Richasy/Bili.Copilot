// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 搜索侧边栏.
/// </summary>
public sealed partial class SearchSideBody : SearchPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSideBody"/> class.
    /// </summary>
    public SearchSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

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
        var item = sender.SelectedItem as ISearchSectionDetailViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.SelectedSection is not null)
        {
            SectionView.Select(ViewModel.Sections.ToList().IndexOf(ViewModel.SelectedSection));
        }
    }
}
