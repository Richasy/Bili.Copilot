// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;

namespace BiliCopilot.UI.Controls.WebDav;

/// <summary>
/// WebDAV 播放器侧边栏主体.
/// </summary>
public sealed partial class WebDavPlayerSideBody : WebDavPlayerPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerSideBody"/> class.
    /// </summary>
    public WebDavPlayerSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
        => await CheckSelectedItemAsync();

    private void OnVideoSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as WebDavStorageItemViewModel;
        if (item is not null && ViewModel.Current != item)
        {
            ViewModel.InitializeCommand.Execute(item.Data);
        }
    }

    private async Task CheckSelectedItemAsync()
    {
        await Task.Delay(100);
        if (ViewModel.Current is null)
        {
            return;
        }

        var index = ViewModel.Playlist.ToList().IndexOf(ViewModel.Current);
        View.Select(index);
        View.StartBringItemIntoView(index, new BringIntoViewOptions { VerticalAlignmentRatio = 0.5 });
    }
}
