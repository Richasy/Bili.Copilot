// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Article;

/// <summary>
/// 文章阅读器页面控件基类.
/// </summary>
public abstract class ArticleReaderPageControlBase : LayoutUserControlBase<ArticleReaderPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReaderPageControlBase"/> class.
    /// </summary>
    protected ArticleReaderPageControlBase() => ViewModel = this.Get<ArticleReaderPageViewModel>();
}
