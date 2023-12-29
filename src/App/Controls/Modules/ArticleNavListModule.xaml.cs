// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 文章导航列表模块.
/// </summary>
public sealed partial class ArticleNavListModule : ArticleNavListModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleNavListModule"/> class.
    /// </summary>
    public ArticleNavListModule()
    {
        InitializeComponent();
        ViewModel = ArticlePageViewModel.Instance;
    }

    private void OnPartitionItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        var item = (PartitionItemViewModel)args.InvokedItem;
        ViewModel.SelectPartitionCommand.Execute(item);
    }
}

/// <summary>
/// <see cref="ArticleNavListModule"/> 的基类.
/// </summary>
public abstract class ArticleNavListModuleBase : ReactiveUserControl<ArticlePageViewModel>
{
}
