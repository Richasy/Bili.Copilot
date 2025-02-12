// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 固定器.
/// </summary>
public sealed partial class Pinner : PinnerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pinner"/> class.
    /// </summary>
    public Pinner() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ViewModel.InitializeCommand.Execute(default);

    private void OnItemClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as FrameworkElement)?.DataContext as PinItem;
        var navVM = this.Get<NavigationViewModel>();
        if (item.Type == Models.Constants.PinContentType.User)
        {
            var user = new UserProfile(item.Id, item.Title, new BiliImage(new Uri(item.Cover)));
            navVM.NavigateToOver(typeof(UserSpacePage), user);
        }
        else if (item.Type == Models.Constants.PinContentType.Pgc)
        {
            var identifier = new MediaIdentifier(item.Id, item.Title, default);
            navVM.NavigateToOver(typeof(PgcPlayerPage), identifier);
        }
        else if (item.Type == Models.Constants.PinContentType.Video)
        {
            var snapshot = new VideoSnapshot(new VideoInformation(new MediaIdentifier(item.Id, item.Title, default), default));
            navVM.NavigateToOver(typeof(VideoPlayerPage), snapshot);
        }
        else if (item.Type == Models.Constants.PinContentType.Live)
        {
            var identifier = new MediaIdentifier(item.Id, item.Title, default);
            navVM.NavigateToOver(typeof(LivePlayerPage), identifier);
        }
        else if (item.Type == Models.Constants.PinContentType.Article)
        {
            var identifier = new ArticleIdentifier(item.Id, item.Title, default, default);
            navVM.NavigateToOver(typeof(ArticleReaderPage), identifier);
        }

        ItemsFlyout.Hide();
    }

    private void OnRemoveItemClick(object sender, RoutedEventArgs e)
    {
        var isLastItem = ViewModel.Items.Count == 1;
        ViewModel.RemoveItemCommand.Execute((sender as FrameworkElement)?.DataContext as PinItem);
        if (isLastItem)
        {
            ItemsFlyout.Hide();
        }
    }
}

/// <summary>
/// 固定器基类.
/// </summary>
public abstract class PinnerBase : LayoutUserControlBase<PinnerViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PinnerBase"/> class.
    /// </summary>
    protected PinnerBase() => ViewModel = this.Get<PinnerViewModel>();
}
