// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 视频卡片控件.
/// </summary>
public sealed partial class VideoCardControl : LayoutControlBase<VideoItemViewModel>
{
    private CardControl _rootCard;
    private Button _userButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoCardControl"/> class.
    /// </summary>
    public VideoCardControl() => DefaultStyleKey = typeof(VideoCardControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootCard = GetTemplateChild("RootCard") as CardControl;
        _userButton = GetTemplateChild("UserButton") as Button;
        if (ViewModel is not null)
        {
            if (_rootCard is not null)
            {
                _rootCard.Command = ViewModel.PlayCommand;
            }

            if (_userButton is not null)
            {
                _userButton.Command = ViewModel.ShowUserSpaceCommand;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ContextRequested += OnContextRequested;

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ContextRequested -= OnContextRequested;

    /// <inheritdoc/>
    protected override void OnViewModelChanged(VideoItemViewModel? oldValue, VideoItemViewModel? newValue)
    {
        if (_rootCard is not null)
        {
            _rootCard.Command = newValue?.PlayCommand;
        }

        if (_userButton is not null)
        {
            _userButton.Command = newValue?.ShowUserSpaceCommand;
        }
    }

    private static MenuFlyoutItem CreateOpenInNewWindowItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.OpenInNewWindow),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.WindowPlay },
            Tag = nameof(ViewModel.OpenInNewWindowCommand),
        };
    }

    private static MenuFlyoutItem CreatePrivatePlayItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.PlayInPrivate),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.EyeOff },
            Tag = nameof(ViewModel.PlayInPrivateCommand),
        };
    }

    private static MenuFlyoutItem CreateUserSpaceItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.EnterUserSpace),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Person },
            Tag = nameof(ViewModel.ShowUserSpaceCommand),
        };
    }

    private static MenuFlyoutItem CreateAddViewLaterItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.AddToViewLater),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.CalendarAdd },
            Tag = nameof(ViewModel.AddToViewLaterCommand),
        };
    }

    private MenuFlyoutItem CreateRemoveViewLaterItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Remove),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.RemoveViewLaterCommand),
        };
    }

    private MenuFlyoutItem CreateRemoveHistoryItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Remove),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.RemoveHistoryCommand),
        };
    }

    private MenuFlyoutItem CreateRemoveFavoriteItem()
    {
        return new MenuFlyoutItem() { Text = ResourceToolkit.GetLocalizedString(StringNames.Remove), Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") }, Tag = nameof(ViewModel.RemoveFavoriteCommand) };
    }

    private static MenuFlyoutItem CreateOpenInBroswerItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.OpenInBrowser),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Globe },
            Tag = nameof(ViewModel.OpenInBroswerCommand),
        };
    }

    private static MenuFlyoutItem CreateCopyUriItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.CopyVideoUrl),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Copy },
            Tag = nameof(ViewModel.CopyUriCommand),
        };
    }

    private static MenuFlyoutItem CreatePinItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.FixContent),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Pin },
            Tag = nameof(ViewModel.PinCommand),
        };
    }

    private void OnContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        if (ContextFlyout is null)
        {
            CreateContextFlyout();
            args.TryGetPosition(this, out var point);
            ContextFlyout.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions { Position = point });
        }

        RelocateCommands();
        args.Handled = true;
    }

    private void CreateContextFlyout()
    {
        var menuFlyout = new MenuFlyout() { ShouldConstrainToRootBounds = false };
        menuFlyout.Items.Add(CreatePrivatePlayItem());
        menuFlyout.Items.Add(CreateOpenInNewWindowItem());
        if (ViewModel.Style != VideoCardStyle.Moment && ViewModel.IsUserValid)
        {
            menuFlyout.Items.Add(CreateUserSpaceItem());
        }

        if (ViewModel.Style != VideoCardStyle.ViewLater)
        {
            menuFlyout.Items.Add(CreateAddViewLaterItem());
        }

        menuFlyout.Items.Add(CreateOpenInBroswerItem());
        menuFlyout.Items.Add(CreateCopyUriItem());
        menuFlyout.Items.Add(CreatePinItem());
        if (ViewModel.Style == VideoCardStyle.ViewLater)
        {
            menuFlyout.Items.Add(CreateRemoveViewLaterItem());
        }
        else if (ViewModel.Style == VideoCardStyle.History)
        {
            menuFlyout.Items.Add(CreateRemoveHistoryItem());
        }
        else if (ViewModel.Style == VideoCardStyle.Favorite)
        {
            menuFlyout.Items.Add(CreateRemoveFavoriteItem());
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
                case nameof(ViewModel.PlayInPrivateCommand):
                    item.Command = ViewModel.PlayInPrivateCommand;
                    break;
                case nameof(ViewModel.ShowUserSpaceCommand):
                    item.Command = ViewModel.ShowUserSpaceCommand;
                    break;
                case nameof(ViewModel.AddToViewLaterCommand):
                    item.Command = ViewModel.AddToViewLaterCommand;
                    break;
                case nameof(ViewModel.RemoveViewLaterCommand):
                    item.Command = ViewModel.RemoveViewLaterCommand;
                    break;
                case nameof(ViewModel.RemoveHistoryCommand):
                    item.Command = ViewModel.RemoveHistoryCommand;
                    break;
                case nameof(ViewModel.RemoveFavoriteCommand):
                    item.Command = ViewModel.RemoveFavoriteCommand;
                    break;
                case nameof(ViewModel.OpenInBroswerCommand):
                    item.Command = ViewModel.OpenInBroswerCommand;
                    break;
                case nameof(ViewModel.CopyUriCommand):
                    item.Command = ViewModel.CopyUriCommand;
                    break;
                case nameof(ViewModel.OpenInNewWindowCommand):
                    item.Command = ViewModel.OpenInNewWindowCommand;
                    break;
                case nameof(ViewModel.PinCommand):
                    item.Command = ViewModel.PinCommand;
                    break;
                default:
                    break;
            }
        }
    }
}
