// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// PGC播放页信息区块详情视图模型.
/// </summary>
public sealed partial class PgcPlayerInfoSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerInfoSectionDetailViewModel"/> class.
    /// </summary>
    public PgcPlayerInfoSectionDetailViewModel(PgcPlayerPageViewModel pageVM)
        => Page = pageVM;

    /// <summary>
    /// 视频播放页视图模型.
    /// </summary>
    public PgcPlayerPageViewModel Page { get; }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Information);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;
}
