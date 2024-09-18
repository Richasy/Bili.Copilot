// Copyright (c) Bili Copilot. All rights reserved.

using System.ComponentModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 用户卡片.
/// </summary>
public sealed partial class UserCardControl : LayoutControlBase<UserItemViewModel>
{
    private CardControl _rootCard;
    private ToggleButton _followButton;
    private FluentIcons.WinUI.SymbolIcon _followIcon;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCardControl"/> class.
    /// </summary>
    public UserCardControl() => DefaultStyleKey = typeof(UserCardControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootCard = GetTemplateChild("RootCard") as CardControl;
        _followButton = GetTemplateChild("FollowButton") as ToggleButton;
        _followIcon = GetTemplateChild("FollowIcon") as FluentIcons.WinUI.SymbolIcon;
        if (_rootCard is not null)
        {
            _rootCard.Command = ViewModel?.ShowUserSpaceCommand;
        }

        if (_followButton is not null)
        {
            _followButton.Command = ViewModel?.ToggleFollowCommand;
        }

        CheckFollowState();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ContextRequested += OnContextRequested;
        CheckFollowState();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ContextRequested -= OnContextRequested;
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(UserItemViewModel? oldValue, UserItemViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (newValue is not null)
        {
            newValue.PropertyChanged += OnViewModelPropertyChanged;
        }

        if (_rootCard is not null)
        {
            _rootCard.Command = newValue?.ShowUserSpaceCommand;
        }

        if (_followButton is not null)
        {
            _followButton.Command = newValue?.ToggleFollowCommand;
        }
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

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsFollowed))
        {
            CheckFollowState();
        }
    }

    private void CheckFollowState()
    {
        if (_followButton is not null)
        {
            _followButton.IsChecked = ViewModel?.IsFollowed;
        }

        if (_followIcon is not null)
        {
            _followIcon.IconVariant = ViewModel?.IsFollowed == true ? FluentIcons.Common.IconVariant.Filled : FluentIcons.Common.IconVariant.Regular;
        }
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
        menuFlyout.Items.Add(CreatePinItem());
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
                case nameof(ViewModel.PinCommand):
                    item.Command = ViewModel.PinCommand;
                    break;
            }
        }
    }
}
