// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 播放器通知控件.
/// </summary>
public sealed partial class PlayerNotificationControl : PlayerNotificationControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotificationControl"/> class.
    /// </summary>
    public PlayerNotificationControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ViewModel.StartCommand.Execute(default);

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.CancelCommand.Execute(default);
}

/// <summary>
/// 播放器通知控件基类.
/// </summary>
public abstract class PlayerNotificationControlBase : LayoutUserControlBase<PlayerNotificationItemViewModel>
{
}
