// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 账户卡片.
/// </summary>
public sealed partial class AccountCard : HeaderAccountControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountCard"/> class.
    /// </summary>
    public AccountCard() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);
}
