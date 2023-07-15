// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Community;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 横幅视图模型.
/// </summary>
public sealed partial class BannerViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BannerViewModel"/> class.
    /// </summary>
    /// <param name="data">横幅数据.</param>
    public BannerViewModel(BannerIdentifier data)
    {
        Uri = data.Uri;
        Description = data.Title;
        IsTooltipEnabled = !string.IsNullOrEmpty(Description);
        Cover = data.Image.Uri;
        MinHeight = 100d;
    }
}
