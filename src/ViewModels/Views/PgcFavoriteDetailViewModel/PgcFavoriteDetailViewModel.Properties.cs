// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 追番/追剧详情视图模型.
/// </summary>
public partial class PgcFavoriteDetailViewModel
{
    private readonly FavoriteType _type;

    private bool _isEnd;

    [ObservableProperty]
    private int _status;

    [ObservableProperty]
    private bool _isEmpty;
}
