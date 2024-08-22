// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 视频卡片控件.
/// </summary>
public sealed class VideoCardControl : LayoutControlBase<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCardControl"/> class.
    /// </summary>
    public VideoCardControl()
    {
        DefaultStyleKey = typeof(VideoCardControl);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ContextRequested += OnContextRequested;

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ContextRequested -= OnContextRequested;

    private static MenuFlyoutItem CreatePrivatePlayItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.PlayInPrivate),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.EyeOff },
            Tag = "PrivatePlay",
        };
    }

    private static MenuFlyoutItem CreateUserSpaceItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.EnterUserSpace),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Person },
            Tag = "UserSpace",
        };
    }

    private static MenuFlyoutItem CreateAddViewLaterItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.AddToViewLater),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.CalendarAdd },
            Tag = "AddViewLater",
        };
    }

    private static MenuFlyoutItem CreateRemoveViewLaterItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Remove),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = ResourceToolkit.GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = "RemoveViewLater",
        };
    }

    private static MenuFlyoutItem CreateOpenInBroswerItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.OpenInBrowser),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Globe },
            Tag = "OpenInBrowser",
        };
    }

    private static MenuFlyoutItem CreateCopyUriItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.CopyVideoUrl),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Copy },
            Tag = "CopyUri",
        };
    }

    private void OnContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (ContextFlyout is null)
        {
            CreateContextFlyout();
            ContextFlyout.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions { Position = new Point(ActualWidth / 2, ActualHeight / 2) });
        }

        RelocateCommands();
    }

    private void CreateContextFlyout()
    {
        var menuFlyout = new MenuFlyout() { ShouldConstrainToRootBounds = false };
        menuFlyout.Items.Add(CreatePrivatePlayItem());
        if (ViewModel.Style != VideoCardStyle.Moment)
        {
            menuFlyout.Items.Add(CreateUserSpaceItem());
        }

        if (ViewModel.Style != VideoCardStyle.ViewLater)
        {
            menuFlyout.Items.Add(CreateAddViewLaterItem());
        }

        menuFlyout.Items.Add(CreateOpenInBroswerItem());
        menuFlyout.Items.Add(CreateCopyUriItem());
        if (ViewModel.Style == VideoCardStyle.ViewLater)
        {
            menuFlyout.Items.Add(CreateRemoveViewLaterItem());
        }

        ContextFlyout = menuFlyout;
    }

    private void RelocateCommands()
    {
        if (ContextFlyout is not MenuFlyout flyout)
        {
            return;
        }

        foreach (var item in flyout.Items.OfType<MenuFlyoutItem>())
        {
            switch (item.Tag.ToString())
            {
                case "PrivatePlay":
                    item.Command = ViewModel.PlayInPrivateCommand;
                    break;
                case "UserSpace":
                    item.Command = ViewModel.ShowUserSpaceCommand;
                    break;
                case "AddViewLater":
                    item.Command = ViewModel.AddToViewLaterCommand;
                    break;
                case "RemoveViewLater":
                    item.Command = ViewModel.RemoveViewLaterCommand;
                    break;
                case "OpenInBrowser":
                    item.Command = ViewModel.OpenInBroswerCommand;
                    break;
                case "CopyUri":
                    item.Command = ViewModel.CopyUriCommand;
                    break;
                default:
                    break;
            }
        }
    }
}
