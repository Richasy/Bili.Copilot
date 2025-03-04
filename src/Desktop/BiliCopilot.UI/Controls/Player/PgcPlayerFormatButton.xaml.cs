// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// PGC播放器格式按钮.
/// </summary>
public sealed partial class PgcPlayerFormatButton : PgcPlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerFormatButton"/> class.
    /// </summary>
    public PgcPlayerFormatButton() => InitializeComponent();

    private void OnFlyoutOpened(object sender, object e)
    {
        var index = ViewModel.Formats.ToList().IndexOf(ViewModel.SelectedFormat);
        if (index != -1)
        {
            FormatView.Select(index);
        }
    }

    private void OnFormatSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var format = sender.SelectedItem as PlayerFormatItemViewModel;
        ViewModel.ChangeFormatCommand.Execute(format);
    }
}
