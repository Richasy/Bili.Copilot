﻿// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Automation.Peers;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// <see cref="CardPanel"/>的自动化公开属性.
/// </summary>
public class CardPanelAutomationPeer : ToggleButtonAutomationPeer
{
    private readonly CardPanel _owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="CardPanelAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">对象.</param>
    public CardPanelAutomationPeer(CardPanel owner)
        : base(owner) => _owner = owner;

    /// <inheritdoc/>
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ListItem;

    /// <inheritdoc/>
    protected override int GetPositionInSetCore()
    {
        var element = _owner as FrameworkElement;
        var parent = _owner.Parent;
        if (parent is not ItemsRepeater and not null)
        {
            parent = (parent as FrameworkElement).Parent;
            element = _owner.Parent as FrameworkElement;
        }

        return (parent as ItemsRepeater)?.GetElementIndex(element) + 1
            ?? base.GetPositionInSetCore();
    }

    /// <inheritdoc/>
    protected override int GetSizeOfSetCore()
    {
        var parent = _owner.Parent;
        if (parent is not ItemsRepeater and not null)
        {
            parent = (parent as FrameworkElement).Parent;
        }

        var count = (parent as ItemsRepeater)?.ItemsSourceView?.Count;
        return count ?? base.GetSizeOfSetCore();
    }
}
