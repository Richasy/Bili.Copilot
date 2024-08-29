// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// 收藏页面控件基类.
/// </summary>
public abstract class FavoritesPageControlBase : LayoutUserControlBase<FavoritesPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPageControlBase"/> class.
    /// </summary>
    protected FavoritesPageControlBase() => ViewModel = this.Get<FavoritesPageViewModel>();
}
