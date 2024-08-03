// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 直播条目视图模型.
/// </summary>
public sealed partial class LiveItemViewModel : ViewModelBase<LiveInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveItemViewModel"/> class.
    /// </summary>
    public LiveItemViewModel(LiveInformation data)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover.Uri;
        Author = data.User?.Name;
        Avatar = data.User?.Avatar?.Uri;
        ViewerCount = data.GetExtensionIfNotNull<double>(LiveExtensionDataId.ViewerCount);
        Subtitle = data.GetExtensionIfNotNull<string>(VideoExtensionDataId.Subtitle);
    }
}
