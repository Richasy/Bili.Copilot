// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 评论页面.
/// </summary>
public sealed partial class CommentPage : CommentPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentPage"/> class.
    /// </summary>
    public CommentPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is CommentModuleViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}

/// <summary>
/// <see cref="CommentPage"/> 的基类.
/// </summary>
public abstract class CommentPageBase : PageBase<CommentModuleViewModel>
{
}
