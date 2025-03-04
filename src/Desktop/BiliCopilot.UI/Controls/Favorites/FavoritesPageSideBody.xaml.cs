// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// 收藏页面侧边栏主体.
/// </summary>
public sealed partial class FavoritesPageSideBody : FavoritesPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPageSideBody"/> class.
    /// </summary>
    public FavoritesPageSideBody() => InitializeComponent();

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
        var item = sender.SelectedItem as IFavoriteSectionDetailViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }

    private void CheckSectionSelection()
    {
        if (ViewModel.CurrentSection is not null)
        {
            SectionView.Select(ViewModel.Sections.IndexOf(ViewModel.CurrentSection));
        }
    }
}
