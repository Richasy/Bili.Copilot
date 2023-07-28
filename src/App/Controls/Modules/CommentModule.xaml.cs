// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 评论模块视图.
/// </summary>
public sealed partial class CommentModule : CommentModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentModule"/> class.
    /// </summary>
    public CommentModule() => InitializeComponent();
}

/// <summary>
/// <see cref="CommentModule"/> 的基类.
/// </summary>
public abstract class CommentModuleBase : ReactiveUserControl<CommentModuleViewModel>
{
}
