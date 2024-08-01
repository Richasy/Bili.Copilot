﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Anime;

/// <summary>
/// 动漫页面控件基类.
/// </summary>
public abstract class AnimePageControlBase : LayoutUserControlBase<AnimePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimePageControlBase"/> class.
    /// </summary>
    protected AnimePageControlBase() => ViewModel = this.Get<AnimePageViewModel>();
}
