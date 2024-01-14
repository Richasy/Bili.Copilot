// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// WebDAV 播放器页面.
/// </summary>
public sealed partial class WebDavPlayerPage : WebDavPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPage"/> class.
    /// </summary>
    public WebDavPlayerPage()
    {
        InitializeComponent();
        ViewModel = new WebDavPlayerPageViewModel();
        DataContext = ViewModel;
    }

    /// <inheritdoc/>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is List<WebDavStorageItemViewModel> items)
        {
            ViewModel.SetWindow(Frame.Tag);

            var selectedItem = items.FirstOrDefault(p => p.IsSelected);
            var index = items.IndexOf(selectedItem);
            var dataList = items.Select(p => p.Data).ToList();
            await ViewModel.SetPlaylistAsync(dataList, index);
        }
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
    {
        try
        {
            ViewModel.PlayerDetail.Player.Dispose();
        }
        catch (Exception)
        {
        }

        ViewModel?.Dispose();
        ViewModel = null;
    }

    private void OnSectionHeaderItemInvoked(object sender, Models.App.Other.PlayerSectionHeader e)
    {
        if (ViewModel.CurrentSection != e)
        {
            ViewModel.CurrentSection = e;
        }
    }
}

/// <summary>
/// WebDAV 播放器页面基类.
/// </summary>
public abstract class WebDavPlayerPageBase : PageBase<WebDavPlayerPageViewModel>
{
}
