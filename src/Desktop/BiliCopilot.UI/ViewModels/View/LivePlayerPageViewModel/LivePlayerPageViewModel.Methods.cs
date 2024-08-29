// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 直播播放页视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    private void InitializeView(LivePlayerView view)
    {
        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
        Player.Title = Title;
        Description = view.Information.GetExtensionIfNotNull<string>(LiveExtensionDataId.Description);
        UpName = view.Information.User.Name;
        UpAvatar = view.Information.User.Avatar.Uri;
        StartTime = view.Information.GetExtensionIfNotNull<DateTimeOffset>(LiveExtensionDataId.StartTime);
        StartRelativeTime = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.LiveStartTimeTemplate), StartTime.Humanize(default, new System.Globalization.CultureInfo("zh-CN")));
        TagName = view.Tag.Name;
        RoomId = view.Information.Identifier.Id;
        ViewerCount = view.Information.GetExtensionIfNotNull<int>(LiveExtensionDataId.ViewerCount);
        Duration = Convert.ToInt32((DateTimeOffset.Now - StartTime).TotalSeconds);
        CalcPlayerHeight();
    }

    private void InitializeLiveMedia(LiveMediaInformation info)
    {
        Formats = info.Formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();
        Lines = info.Lines.ToList()
            .Where(p => p.Urls.FirstOrDefault()?.Protocol == "http_hls" || p.Urls.FirstOrDefault()?.Protocol == "http_stream")
            .ToList();

        var currentQuality = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastSelectedLiveQuality, 400);
        var format = Formats.FirstOrDefault(p => p.Data.Quality == currentQuality);
        SelectedFormat = format ?? Formats.FirstOrDefault();
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        Cover = default;
        Title = default;
        Description = default;
        UpName = default;
        UpAvatar = default;
        StartRelativeTime = default;
        IsFollow = default;
        TagName = default;
        RoomId = default;
        ViewerCount = default;

        Formats = default;
        SelectedFormat = default;
        Lines = default;
        SelectedLine = default;
    }

    private void CalcPlayerHeight()
    {
        if (PlayerWidth <= 0 || Player.IsFullScreen || Player.IsCompactOverlay)
        {
            return;
        }

        PlayerHeight = (double)(PlayerWidth * 9 / 16);
    }

    private void ReloadFormat()
    {
        if (SelectedFormat is null)
        {
            return;
        }

        ChangeFormatCommand.Execute(SelectedFormat);
    }
}
