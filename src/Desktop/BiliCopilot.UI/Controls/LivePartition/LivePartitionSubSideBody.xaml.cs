// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播分区页面侧边栏.
/// </summary>
public sealed partial class LivePartitionSubSideBody : LivePartitionPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePartitionSubSideBody"/> class.
    /// </summary>
    public LivePartitionSubSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.MainSectionChanged += OnSectionChanged;
        CheckPartitionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.MainSectionChanged -= OnSectionChanged;

    private void OnSectionChanged(object? sender, EventArgs e)
        => CheckPartitionSelection();

    private void OnPartitionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var partition = sender.SelectedItem as PartitionViewModel;
        if (partition is not null)
        {
            ViewModel.SelectSubSectionCommand.Execute(partition);
        }
    }

    private void CheckPartitionSelection()
    {
        if (ViewModel.SelectedSubSection is not null && ViewModel.SubPartitions is not null)
        {
            var partition = ViewModel.SubPartitions.FirstOrDefault(p => p.Equals(ViewModel.SelectedSubSection));
            var index = ViewModel.SubPartitions.ToList().IndexOf(partition);
            PartitionView.Select(index);
        }
    }
}
