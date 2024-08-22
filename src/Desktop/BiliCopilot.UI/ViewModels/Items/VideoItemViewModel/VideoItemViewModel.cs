// Copyright (c) Bili Copilot. All rights reserved.

using System.Globalization;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Globalization;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频项视图模型.
/// </summary>
public sealed partial class VideoItemViewModel : ViewModelBase<VideoInformation>
{
    private readonly Action<VideoItemViewModel>? _removeViewLaterAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoItemViewModel"/> class.
    /// </summary>
    public VideoItemViewModel(
        VideoInformation info,
        VideoCardStyle style,
        Action<VideoItemViewModel> removeViewLaterAction = default)
        : base(info)
    {
        _removeViewLaterAction = removeViewLaterAction;
        var primaryLan = ApplicationLanguages.Languages[0];
        Style = style;
        Title = info.Identifier.Title;
        Cover = info.Identifier.Cover.Uri;
        Author = info.Publisher?.User?.Name;
        Avatar = info.Publisher?.User?.Avatar?.Uri;
        Duration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(info.Duration ?? 0));
        PublishRelativeTime = info.PublishTime?.Humanize(culture: new CultureInfo(primaryLan));
        PlayCount = info.CommunityInformation?.PlayCount;
        DanmakuCount = info.CommunityInformation?.DanmakuCount;
        LikeCount = info.CommunityInformation?.LikeCount;
        TagName = info.GetExtensionIfNotNull<string?>(VideoExtensionDataId.TagName);
        RecommendReason = info.GetExtensionIfNotNull<string?>(VideoExtensionDataId.RecommendReason);
        Subtitle = info.GetExtensionIfNotNull<string?>(VideoExtensionDataId.Subtitle);
        CollectTime = info.GetExtensionIfNotNull<DateTimeOffset>(VideoExtensionDataId.CollectTime).Humanize(default, new CultureInfo("zh-CN"));
        var progress = info.GetExtensionIfNotNull<int?>(VideoExtensionDataId.Progress);
        if (progress is not null)
        {
            ProgressText = AppToolkit.FormatDuration(TimeSpan.FromSeconds(progress.Value));
        }
    }

    [RelayCommand]
    private void Play()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage).FullName, new VideoSnapshot(Data));

    [RelayCommand]
    private void PlayInPrivate()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage).FullName, new VideoSnapshot(Data, true));

    [RelayCommand]
    private void ShowUserSpace()
    {
        if (Data.Publisher?.User is not null)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage).FullName, Data.Publisher.User);
        }
    }

    [RelayCommand]
    private async Task AddToViewLaterAsync()
    {
        try
        {
            await this.Get<IViewLaterService>().AddAsync(Data.Identifier.Id);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterSucceed), InfoType.Success));
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoItemViewModel>>().LogError(ex, "添加稍后再看失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterFailed), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task RemoveViewLaterAsync()
    {
        try
        {
            await this.Get<IViewLaterService>().AddAsync(Data.Identifier.Id);
            _removeViewLaterAction?.Invoke(this);
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoItemViewModel>>().LogError(ex, "移除视频失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToRemoveVideoFromViewLater), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
        => await Windows.System.Launcher.LaunchUriAsync(GetWebUri()).AsTask();

    [RelayCommand]
    private void CopyUri()
    {
        var dp = new DataPackage();
        dp.SetText(GetWebUri().ToString());
        dp.SetWebLink(GetWebUri());
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    private Uri GetWebUri()
    {
        var shortLink = Data.GetExtensionIfNotNull<string>(VideoExtensionDataId.ShortLink);
        return string.IsNullOrEmpty(shortLink) ? new Uri($"https://www.bilibili.com/video/av{Data.Identifier.Id}") : new Uri(shortLink);
    }
}
