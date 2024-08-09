// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Article;

/// <summary>
/// 文章阅读器工具栏.
/// </summary>
public sealed partial class ArticleReaderToolBar : ArticleReaderToolBarBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReaderToolBar"/> class.
    /// </summary>
    public ArticleReaderToolBar() => InitializeComponent();
}

/// <summary>
/// 文章阅读器工具栏基类.
/// </summary>
public abstract class ArticleReaderToolBarBase : LayoutUserControlBase<ArticleReaderPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReaderToolBarBase"/> class.
    /// </summary>
    protected ArticleReaderToolBarBase() => ViewModel = this.Get<ArticleReaderPageViewModel>();
}
