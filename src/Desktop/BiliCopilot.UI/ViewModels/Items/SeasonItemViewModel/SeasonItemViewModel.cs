// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 剧集条目视图模型.
/// </summary>
public sealed partial class SeasonItemViewModel : ViewModelBase<SeasonInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonItemViewModel"/> class.
    /// </summary>
    public SeasonItemViewModel(SeasonInformation data)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover.Uri;
        Subtitle = data.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Subtitle);
        Highlight = data.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Highlight);
        Score = data.GetExtensionIfNotNull<double>(SeasonExtensionDataId.Score);
    }

    [RelayCommand]
    private void Play()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(PgcPlayerPage).FullName, Data.Identifier);
}
