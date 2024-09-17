// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 聊天模型卡片控件.
/// </summary>
public sealed partial class ChatModelCardControl : LayoutControlBase<ChatModelItemViewModel>
{
    private Button _moreButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelCardControl"/> class.
    /// </summary>
    public ChatModelCardControl() => DefaultStyleKey = typeof(ChatModelCardControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _moreButton = GetTemplateChild("MoreButton") as Button;
        if (_moreButton is not null)
        {
            _moreButton.Flyout = CreateMoreFlyout();
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatModelItemViewModel? oldValue, ChatModelItemViewModel? newValue)
    {
        if (_moreButton is not null && newValue is not null && _moreButton.Flyout is MenuFlyout mf)
        {
            foreach (var item in mf.Items.OfType<MenuFlyoutItem>())
            {
                if (item.Tag.ToString() == nameof(ViewModel.ModifyCommand))
                {
                    item.Command = newValue.ModifyCommand;
                }
                else if (item.Tag.ToString() == nameof(ViewModel.DeleteCommand))
                {
                    item.Command = newValue.DeleteCommand;
                }
            }
        }
    }

    private MenuFlyout CreateMoreFlyout()
    {
        var flyout = new MenuFlyout();
        flyout.Items.Add(new MenuFlyoutItem
        {
            Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Modify),
            MinWidth = 160,
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Edit },
            Tag = nameof(ViewModel.ModifyCommand),
            Command = ViewModel.ModifyCommand,
        });

        flyout.Items.Add(new MenuFlyoutItem
        {
            Text = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Delete),
            MinWidth = 160,
            Icon = new FluentIcons.WinUI.SymbolIcon { Symbol = FluentIcons.Common.Symbol.Delete, Foreground = ResourceToolkit.GetThemeBrush("SystemFillColorCriticalBrush") },
            Tag = nameof(ViewModel.DeleteCommand),
            Command = ViewModel.DeleteCommand,
        });

        return flyout;
    }
}
