// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC推荐内容详情视图模型.
/// </summary>
public partial class PgcRecommendDetailViewModel
{
    private readonly PgcType _type;
    private bool _isEnd;

    [ObservableProperty]
    private bool _isEmpty;
}
