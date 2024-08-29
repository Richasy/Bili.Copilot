// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播分区主体侧边栏.
/// </summary>
public sealed partial class LivePartitionMainSideBody : LivePartitionPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionMainSideBody"/> class.
    /// </summary>
    public LivePartitionMainSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.SectionInitialized += OnSectionInitialized;
        CheckPartitionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.SectionInitialized -= OnSectionInitialized;

    private void OnSectionInitialized(object? sender, EventArgs e)
        => CheckPartitionSelection();

    private void OnPartitionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var partition = sender.SelectedItem as PartitionViewModel;
        if (partition is not null)
        {
            ViewModel.SelectMainSectionCommand.Execute(partition);
        }
    }

    private void CheckPartitionSelection()
    {
        if (ViewModel.SelectedMainSection is not null)
        {
            var partition = ViewModel.MainSections.FirstOrDefault(p => p.Equals(ViewModel.SelectedMainSection));
            PartitionView.Select(ViewModel.MainSections.IndexOf(partition));
        }
    }
}
