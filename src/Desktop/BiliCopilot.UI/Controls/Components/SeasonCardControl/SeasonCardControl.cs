// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 剧集卡片展示器.
/// </summary>
public abstract class SeasonCardPresenter : LayoutUserControlBase<SeasonItemViewModel>;

/// <summary>
/// 剧集卡片控件.
/// </summary>
public sealed partial class SeasonCardControl : LayoutControlBase<SeasonItemViewModel>
{
    private ButtonBase _rootCard;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonCardControl"/> class.
    /// </summary>
    public SeasonCardControl() => DefaultStyleKey = typeof(SeasonCardControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootCard = GetTemplateChild("RootCard") as ButtonBase;
        if (ViewModel is not null)
        {
            _rootCard?.Command = ViewModel.PlayCommand;
        }
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ContextRequested += OnContextRequested;

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ContextRequested -= OnContextRequested;

    /// <inheritdoc/>
    protected override void OnViewModelChanged(SeasonItemViewModel? oldValue, SeasonItemViewModel? newValue)
    {
        if (_rootCard is not null)
        {
            _rootCard.Command = newValue?.PlayCommand;
        }
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

    private static MenuFlyoutItem CreateFollowItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Follow),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Heart },
            Tag = nameof(ViewModel.FollowCommand),
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

    private static MenuFlyoutItem CreateMarkWantWatchItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.MarkWantWatch),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Thinking },
            Tag = nameof(ViewModel.MarkWantWatchCommand),
        };
    }

    private static MenuFlyoutItem CreateMarkWatchingItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.MarkWatching),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Eye },
            Tag = nameof(ViewModel.MarkWatchingCommand),
        };
    }

    private static MenuFlyoutItem CreateMarkWatchedItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.MarkWatched),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Checkmark },
            Tag = nameof(ViewModel.MarkWatchedCommand),
        };
    }

    private MenuFlyoutItem CreateUnfollowItem()
    {
        return new MenuFlyoutItem()
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.Unfollow),
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.HeartBroken, Foreground = this.Get<IResourceToolkit>().GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.UnfollowCommand),
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
        menuFlyout.Items.Add(CreateOpenInBroswerItem());
        if (ViewModel.Style != SeasonCardStyle.Favorite)
        {
            menuFlyout.Items.Add(CreateFollowItem());
            menuFlyout.Items.Add(CreatePinItem());
        }
        else
        {
            menuFlyout.Items.Add(CreatePinItem());
            menuFlyout.Items.Add(CreateMarkWantWatchItem());
            menuFlyout.Items.Add(CreateMarkWatchingItem());
            menuFlyout.Items.Add(CreateMarkWatchedItem());
            menuFlyout.Items.Add(CreateUnfollowItem());
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
                case nameof(ViewModel.OpenInBroswerCommand):
                    item.Command = ViewModel.OpenInBroswerCommand;
                    break;
                case nameof(ViewModel.FollowCommand):
                    item.Command = ViewModel.FollowCommand;
                    break;
                case nameof(ViewModel.PinCommand):
                    item.Command = ViewModel.PinCommand;
                    break;
                case nameof(ViewModel.MarkWantWatchCommand):
                    item.IsEnabled = !(ViewModel.InWantWatch ?? false);
                    item.Command = ViewModel.MarkWantWatchCommand;
                    break;
                case nameof(ViewModel.MarkWatchingCommand):
                    item.IsEnabled = !(ViewModel.InWatching ?? false);
                    item.Command = ViewModel.MarkWatchingCommand;
                    break;
                case nameof(ViewModel.MarkWatchedCommand):
                    item.IsEnabled = !(ViewModel.InWatched ?? false);
                    item.Command = ViewModel.MarkWatchedCommand;
                    break;
                case nameof(ViewModel.UnfollowCommand):
                    item.Command = ViewModel.UnfollowCommand;
                    break;
            }
        }
    }
}
