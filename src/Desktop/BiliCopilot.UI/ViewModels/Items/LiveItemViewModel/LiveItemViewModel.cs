// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
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
        TagName = data.GetExtensionIfNotNull<string>(LiveExtensionDataId.TagName);
        IsLiving = data.GetExtensionIfNotNull<bool?>(LiveExtensionDataId.IsLiving);
        var collectTime = data.GetExtensionIfNotNull<DateTimeOffset?>(LiveExtensionDataId.CollectTime);
        if (collectTime is not null)
        {
            CollectRelativeTime = collectTime.Value.Humanize();
        }
    }

    [RelayCommand]
    private void Play()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(LivePlayerPage).FullName, Data);
}
