// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// PGC播放页信息区块详情视图模型.
/// </summary>
public sealed partial class PgcPlayerInfoSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerInfoSectionDetailViewModel"/> class.
    /// </summary>
    public PgcPlayerInfoSectionDetailViewModel(PgcConnectorViewModel pageVM)
        => Page = pageVM;

    /// <summary>
    /// 视频播放页视图模型.
    /// </summary>
    public PgcConnectorViewModel Page { get; }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Information);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;
}
