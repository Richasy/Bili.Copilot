// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Article;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 阅读页视图模型.
/// </summary>
public sealed partial class ReaderPageViewModel
{
    private ArticleInformation _article;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private UserItemViewModel _author;

    [ObservableProperty]
    private CommentModuleViewModel _comments;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isDetailShown;

    [ObservableProperty]
    private bool _isCommentShown;
}
