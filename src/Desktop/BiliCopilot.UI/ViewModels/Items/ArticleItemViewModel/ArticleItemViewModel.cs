// Copyright (c) Bili Copilot. All rights reserved.

using System.Globalization;
using Humanizer;
using Richasy.BiliKernel.Models.Article;
using Richasy.WinUI.Share.ViewModels;
using Windows.Globalization;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 文章项视图模型.
/// </summary>
public sealed partial class ArticleItemViewModel : ViewModelBase<ArticleInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleItemViewModel"/> class.
    /// </summary>
    public ArticleItemViewModel(ArticleInformation data)
        : base(data)
    {
        var primaryLan = ApplicationLanguages.Languages[0];
        Title = data.Identifier.Title.Trim();
        Subtitle = data.GetExtensionIfNotNull<string>(ArticleExtensionDataId.Subtitle);
        Cover = data.Identifier.Cover?.Uri;
        Author = data.Publisher?.Name;
        Avatar = data.Publisher?.Avatar?.Uri;
        PublishRelativeTime = data.PublishDateTime.Humanize(culture: new CultureInfo(primaryLan));
    }
}
