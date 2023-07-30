// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Article;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 阅读页视图模型.
/// </summary>
public sealed partial class ReaderPageViewModel : ViewModelBase
{
    [RelayCommand]
    private void Initialize(ArticleInformation info)
    {
        if (_article == info)
        {
            return;
        }

        IsDetailShown = true;
        _article = info;
        Title = info.Identifier.Title;
        Author = new UserItemViewModel(_article.Publisher);
        Author.InitializeRelationCommand.Execute(default);
        Comments = new CommentModuleViewModel();
        Comments.SetData(info.Identifier.Id, Models.Constants.Bili.CommentType.Article);
    }
}
