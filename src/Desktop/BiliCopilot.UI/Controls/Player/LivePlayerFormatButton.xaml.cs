// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 直播播放器清晰度按钮.
/// </summary>
public sealed partial class LivePlayerFormatButton : LivePlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerFormatButton"/> class.
    /// </summary>
    public LivePlayerFormatButton() => InitializeComponent();

    protected override void OnControlUnloaded()
        => FormatView.ItemsSource = default;

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
        if (format is null || format == ViewModel.SelectedFormat)
        {
            return;
        }

        ViewModel.ChangeFormatCommand.Execute(format);
        QualityFormat.Hide();
    }
}
