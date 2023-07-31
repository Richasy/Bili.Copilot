// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Models.Data.Article;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 文章条目的视图模型.
/// </summary>
public sealed partial class ArticleItemViewModel
{
    private readonly Action<ArticleItemViewModel> _additionalAction;

    [ObservableProperty]
    private ArticleInformation _data;

    [ObservableProperty]
    private UserItemViewModel _publisher;

    [ObservableProperty]
    private string _viewCountText;

    [ObservableProperty]
    private string _likeCountText;

    [ObservableProperty]
    private string _commentCountText;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isShowCommunity;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isAISupported;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ArticleItemViewModel model && EqualityComparer<ArticleInformation>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
