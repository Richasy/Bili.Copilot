// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 动态卡片控件.
/// </summary>
public sealed partial class MomentCardControl : LayoutControlBase<MomentItemViewModel>
{
    private CardControl _rootCard;
    private ButtonBase _userButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="MomentCardControl"/> class.
    /// </summary>
    public MomentCardControl() => DefaultStyleKey = typeof(MomentCardControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootCard = GetTemplateChild("RootCard") as CardControl;
        _userButton = GetTemplateChild("UserButton") as ButtonBase;
        if (ViewModel is not null)
        {
            if (_rootCard is not null)
            {
                _rootCard.Command = ViewModel.ActivateCommand;
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
    protected override void OnViewModelChanged(MomentItemViewModel? oldValue, MomentItemViewModel? newValue)
    {
        if (_rootCard is not null)
        {
            _rootCard.Command = newValue?.ActivateCommand;
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
            Tag = nameof(ViewModel.CopyUrlCommand),
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
        if (ViewModel.Style == MomentCardStyle.Video)
        {
            menuFlyout.Items.Add(CreatePrivatePlayItem());
            menuFlyout.Items.Add(CreateOpenInNewWindowItem());
            menuFlyout.Items.Add(CreateOpenInBroswerItem());
            menuFlyout.Items.Add(CreateUserSpaceItem());
            menuFlyout.Items.Add(CreateAddViewLaterItem());
            menuFlyout.Items.Add(CreateCopyUriItem());
            menuFlyout.Items.Add(CreatePinItem());
        }
        else
        {
            menuFlyout.Items.Add(CreateOpenInBroswerItem());
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
                case nameof(ViewModel.OpenInBroswerCommand):
                    item.Command = ViewModel.OpenInBroswerCommand;
                    break;
                case nameof(ViewModel.CopyUrlCommand):
                    item.Command = ViewModel.CopyUrlCommand;
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
