// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Windows.Foundation;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 文章条目.
/// </summary>
public sealed class ArticleItem : ReactiveControl<ArticleItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleItem"/> class.
    /// </summary>
    public ArticleItem() => DefaultStyleKey = typeof(ArticleItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(400, 240);
}
