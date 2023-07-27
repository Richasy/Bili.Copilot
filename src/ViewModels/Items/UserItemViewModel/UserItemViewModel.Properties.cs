// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.User;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 用户条目视图模型.
/// </summary>
public sealed partial class UserItemViewModel
{
    /// <summary>
    /// 用户基础信息.
    /// </summary>
    [ObservableProperty]
    private UserProfile _user;

    /// <summary>
    /// 用户自我介绍.
    /// </summary>
    [ObservableProperty]
    private string _introduce;

    /// <summary>
    /// 是否为大会员.
    /// </summary>
    [ObservableProperty]
    private bool _isVip;

    /// <summary>
    /// 等级.
    /// </summary>
    [ObservableProperty]
    private int _level;

    /// <summary>
    /// 角色.
    /// </summary>
    [ObservableProperty]
    private string _role;

    /// <summary>
    /// 关注数的可读文本.
    /// </summary>
    [ObservableProperty]
    private string _followCountText;

    /// <summary>
    /// 粉丝数的可读文本.
    /// </summary>
    [ObservableProperty]
    private string _fansCountText;

    /// <summary>
    /// 硬币数的可读文本.
    /// </summary>
    [ObservableProperty]
    private string _coinCountText;

    /// <summary>
    /// 点赞数的可读文本.
    /// </summary>
    [ObservableProperty]
    private string _likeCountText;

    /// <summary>
    /// 与用户的关系.
    /// </summary>
    [ObservableProperty]
    private UserRelationStatus _relation;

    /// <summary>
    /// 是否显示关系按钮.
    /// </summary>
    [ObservableProperty]
    private bool _isRelationButtonShown;

    /// <summary>
    /// 是否正在更改关系.
    /// </summary>
    [ObservableProperty]
    private bool _isRelationChanging;
}
