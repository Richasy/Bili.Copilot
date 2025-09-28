// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.Core;

public sealed partial class ChapterPanel : PlayerControlBase
{
    public ChapterPanel() => InitializeComponent();

    protected override void OnControlLoaded()
    {
        if (ViewModel != null)
        {
            ViewModel.ChapterInitialized -= OnChapterInitialized;
            ViewModel.ChapterInitialized += OnChapterInitialized;
        }
    }

    protected override void OnViewModelChanged(PlayerViewModel? oldValue, PlayerViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.ChapterInitialized -= OnChapterInitialized;
        }

        if (newValue != null)
        {
            newValue.ChapterInitialized += OnChapterInitialized;
            OnChapterInitialized(this, EventArgs.Empty);
        }
    }

    protected override void OnControlUnloaded()
        => ViewModel?.ChapterInitialized -= OnChapterInitialized;

    private void OnChapterInitialized(object? sender, EventArgs e)
    {
        RootGrid.Children.Clear();
        foreach (var item in ViewModel.Chapters)
        {
            var control = ChapterItemTemplate.LoadContent() as FrameworkElement;
            control!.DataContext = item;
            RootGrid.Children.Add(control);
        }

        ResetChapterPosition();
    }

    private async void OnChapterItemClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as FrameworkElement)?.DataContext as ChapterItemViewModel;
        if (item?.Position >= 0)
        {
            await ViewModel.ChangePositionAsync(item.Position);
        }
    }

    private void ResetChapterPosition()
    {
        if (RootGrid.Children.Count != ViewModel.Chapters.Count && ViewModel.Chapters.Count > 0)
        {
            return;
        }

        var index = 0;
        foreach (var item in ViewModel.Chapters)
        {
            var percentage = item.Position / ViewModel.Player.Duration;
            var control = RootGrid.Children[index] as FrameworkElement;
            control?.Margin = new Thickness((percentage * RootGrid.ActualWidth) - 5, 0, 0, 0);
            index++;
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => ResetChapterPosition();
}
