// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 文章分区页面.
/// </summary>
public sealed partial class ArticlePartitionPage : ArticlePartitionPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionPage"/> class.
    /// </summary>
    public ArticlePartitionPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 文章分区页面基类.
/// </summary>
public abstract class ArticlePartitionPageBase : LayoutPageBase<ArticlePartitionPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionPageBase"/> class.
    /// </summary>
    protected ArticlePartitionPageBase() => ViewModel = this.Get<ArticlePartitionPageViewModel>();
}
