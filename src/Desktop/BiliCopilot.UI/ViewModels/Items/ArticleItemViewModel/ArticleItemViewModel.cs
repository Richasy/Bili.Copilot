// Copyright (c) Bili Copilot. All rights reserved.

using System.Globalization;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Article;
using Richasy.WinUI.Share.ViewModels;
using Windows.Globalization;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 文章项视图模型.
/// </summary>
public sealed partial class ArticleItemViewModel : ViewModelBase<ArticleInformation>
{
    private readonly Action<ArticleItemViewModel>? _removeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleItemViewModel"/> class.
    /// </summary>
    public ArticleItemViewModel(ArticleInformation data, Action<ArticleItemViewModel>? removeAction = default)
        : base(data)
    {
        var primaryLan = ApplicationLanguages.Languages[0];
        Title = data.Identifier.Title.Trim();
        Subtitle = data.GetExtensionIfNotNull<string>(ArticleExtensionDataId.Subtitle);
        Cover = data.Identifier.Cover?.Uri;
        Author = data.Publisher?.Name;
        Avatar = data.Publisher?.Avatar?.Uri;
        PublishRelativeTime = data.PublishDateTime.Humanize(culture: new CultureInfo(primaryLan));
        LikeCount = data.CommunityInformation?.LikeCount;
        var collectTime = data.GetExtensionIfNotNull<DateTimeOffset?>(ArticleExtensionDataId.CollectTime);
        if (collectTime is not null)
        {
            CollectRelativeTime = collectTime.Value.Humanize();
        }

        _removeAction = removeAction;
    }

    [RelayCommand]
    private void OpenReader()
    {
        var navVM = this.Get<NavigationViewModel>();
        navVM.NavigateToOver(typeof(ArticleReaderPage).FullName, Data.Identifier);
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage).FullName, Data.Publisher);

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = $"https://www.bilibili.com/read/cv{Data.Identifier.Id}";
        await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
    }

    [RelayCommand]
    private async Task RemoveHistoryAsync()
    {
        try
        {
            await this.Get<IViewHistoryService>().RemoveArticleHistoryItemAsync(Data);
            _removeAction?.Invoke(this);
        }
        catch (Exception ex)
        {
            this.Get<ILogger<ArticleItemViewModel>>().LogError(ex, "移除历史记录时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToRemoveVideoFromHistory), InfoType.Error));
        }
    }
}
