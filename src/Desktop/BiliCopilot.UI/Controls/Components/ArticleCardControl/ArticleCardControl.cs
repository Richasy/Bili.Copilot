// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 文章卡片控件.
/// </summary>
public sealed class ArticleCardControl : LayoutControlBase<ArticleItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleCardControl"/> class.
    /// </summary>
    public ArticleCardControl() => DefaultStyleKey = typeof(ArticleCardControl);
}
