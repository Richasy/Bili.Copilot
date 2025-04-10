// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 直播源视图模型.
/// </summary>
public sealed partial class LiveSourceViewModel
{
    private void InitializeView(LivePlayerView view)
    {
        if (view is null)
        {
            return;
        }

        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
        Description = view.Information.GetExtensionIfNotNull<string>(LiveExtensionDataId.Description);
        UpName = view.Information.User.Name;
        UpAvatar = view.Information.User.Avatar.Uri;
        StartTime = view.Information.GetExtensionIfNotNull<DateTimeOffset>(LiveExtensionDataId.StartTime);
        StartRelativeTime = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.LiveStartTimeTemplate), StartTime.Humanize(default, new System.Globalization.CultureInfo("zh-CN")));
        TagName = view.Tag.Name;
        RoomId = view.Information.Identifier.Id;
        ViewerCount = view.Information.GetExtensionIfNotNull<int>(LiveExtensionDataId.ViewerCount);
        Duration = (DateTimeOffset.Now - StartTime).TotalSeconds;
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
        ErrorMessage = default;

        Formats = default;
        SelectedFormat = default;
        Lines = default;
        SelectedLine = default;
    }

    private void ReloadFormat()
    {
        if (SelectedFormat is null)
        {
            return;
        }

        ChangeFormatCommand.Execute(SelectedFormat);
    }

    private void ScrollMessagesToBottom()
        => Chat.ScrollToBottomCommand.Execute(default);
}
