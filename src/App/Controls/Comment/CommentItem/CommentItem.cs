// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Windows.Foundation;

namespace Bili.Copilot.App.Controls.Comment;

/// <summary>
/// 评论条目.
/// </summary>
public sealed class CommentItem : ReactiveControl<CommentItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentItem"/> class.
    /// </summary>
    public CommentItem() => DefaultStyleKey = typeof(CommentItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(double.PositiveInfinity, 120);
}
