﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 播放器.
/// </summary>
public sealed partial class PlayerPresenter : PlayerPresenterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerPresenter"/> class.
    /// </summary>
    public PlayerPresenter() => InitializeComponent();
}

/// <summary>
/// 播放器基类.
/// </summary>
public abstract class PlayerPresenterBase : LayoutUserControlBase<PlayerViewModelBase>
{
}
