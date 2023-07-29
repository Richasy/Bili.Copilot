// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.User;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 关系用户页面视图模型基类，是粉丝/关注页面的基础.
/// </summary>
public partial class RelationDetailViewModel
{
    private readonly RelationType _relationType;

    private bool _isEnd = false;

    [ObservableProperty]
    private UserProfile _profile;

    [ObservableProperty]
    private string _titleSuffix;

    [ObservableProperty]
    private bool _isEmpty;
}
