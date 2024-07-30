// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

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
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ViewModel.SectionInitialized += OnSectionInitialized;

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.SectionInitialized -= OnSectionInitialized;

    private void OnSectionInitialized(object? sender, EventArgs e)
    {
        if (ViewModel.SelectedSection is not null)
        {
            SectionView.Select(ViewModel.Sections.IndexOf(ViewModel.SelectedSection));
        }
    }

    private void OnSectionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as IPopularSectionItemViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }
}
