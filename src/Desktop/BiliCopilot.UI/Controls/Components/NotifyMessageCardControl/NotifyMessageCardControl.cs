// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 通知消息卡片控件.
/// </summary>
public sealed partial class NotifyMessageCardControl : LayoutControlBase<NotifyMessageItemViewModel>
{
    private CardControl _rootCard;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessageCardControl"/> class.
    /// </summary>
    public NotifyMessageCardControl() => DefaultStyleKey = typeof(NotifyMessageCardControl);

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootCard = GetTemplateChild("RootCard") as CardControl;
        if (ViewModel is not null)
        {
            if (_rootCard is not null)
            {
                _rootCard.Command = ViewModel.ActiveCommand;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(NotifyMessageItemViewModel? oldValue, NotifyMessageItemViewModel? newValue)
    {
        if (_rootCard is not null)
        {
            _rootCard.Command = newValue?.ActiveCommand;
        }
    }
}
