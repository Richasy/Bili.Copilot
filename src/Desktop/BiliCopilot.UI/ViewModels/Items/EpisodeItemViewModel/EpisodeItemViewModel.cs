// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 单集条目视图模型.
/// </summary>
public sealed partial class EpisodeItemViewModel : ViewModelBase<EpisodeInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeItemViewModel"/> class.
    /// </summary>
    public EpisodeItemViewModel(EpisodeInformation data)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover?.Uri;
        Duration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(data.Duration ?? 0));
    }
}
