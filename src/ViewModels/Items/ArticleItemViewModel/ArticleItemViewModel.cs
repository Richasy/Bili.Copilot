// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 文章条目的视图模型.
/// </summary>
public sealed partial class ArticleItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleItemViewModel"/> class.
    /// </summary>
    public ArticleItemViewModel(
        ArticleInformation information,
        Action<ArticleItemViewModel> action = default)
    {
        Data = information;
        _additionalAction = action;
        InitializeData();
        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand);
    }

    /// <summary>
    /// 获取文章详情内容.
    /// </summary>
    /// <returns>文章HTML文本.</returns>
    public async Task<string> GetDetailAsync()
    {
        IsError = false;
        ErrorText = string.Empty;

        if (string.IsNullOrEmpty(_detailContent))
        {
            _detailContent = await ArticleProvider.GetArticleContentAsync(Data.Identifier.Id);
        }

        return _detailContent;
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (!IsReloading)
        {
            _detailContent = string.Empty;
            await GetDetailAsync();
        }
    }

    [RelayCommand]
    private void Read()
        => AppViewModel.Instance.OpenReaderCommand.Execute(Data);

    [RelayCommand]
    private async Task OpenInBrowserAsync()
    {
        var uri = $"https://www.bilibili.com/read/cv{Data.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(uri));
    }

    [RelayCommand]
    private async Task UnfavoriteAsync()
    {
        var result = await FavoriteProvider.RemoveFavoriteArticleAsync(Data.Identifier.Id);
        if (result)
        {
            _additionalAction?.Invoke(this);
        }
    }

    private void DisplayException(Exception exception)
    {
        IsError = true;
        var msg = ResourceToolkit.GetLocalizedString(StringNames.RequestArticleFailed);
        ErrorText = msg;
        LogException(exception);
    }

    private void InitializeData()
    {
        IsShowCommunity = Data.CommunityInformation != null;
        var userVM = new UserItemViewModel(Data.Publisher);
        Publisher = userVM;
        if (IsShowCommunity)
        {
            ViewCountText = NumberToolkit.GetCountText(Data.CommunityInformation.ViewCount);
            CommentCountText = NumberToolkit.GetCountText(Data.CommunityInformation.CommentCount);
            LikeCountText = NumberToolkit.GetCountText(Data.CommunityInformation.LikeCount);
        }
    }
}
