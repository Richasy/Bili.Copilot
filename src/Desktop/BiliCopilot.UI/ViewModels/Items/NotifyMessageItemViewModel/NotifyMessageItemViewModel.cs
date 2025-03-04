// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.System;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 通知消息项视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class NotifyMessageItemViewModel : ViewModelBase<NotifyMessage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessageItemViewModel"/> class.
    /// </summary>
    public NotifyMessageItemViewModel(NotifyMessage data)
        : base(data)
    {
        PublishRelativeTime = data.PublishTime.Humanize(default, new System.Globalization.CultureInfo("zh-CN"));
        FirstUserAvatar = data.Users?.First().Avatar.Uri;
        FirstUserName = data.Users?.First().Name;
        IsMultipleUsers = data.Users.Count > 1;
        Message = data.Message;
        SourceContent = data.SourceContent;
        if (data.Type == NotifyMessageType.Like)
        {
            var count = data.Properties["Count"];
            Subtitle = IsMultipleUsers
                ? string.Format(
                    ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.LikeMessageMultipleDescription),
                    FirstUserName,
                    data.Users[1].Name,
                    count,
                    data.Business)
                : string.Format(
                    ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.LikeMessageSingleDescription),
                    FirstUserName,
                    data.Business);
        }
    }

    [RelayCommand]
    private async Task ActiveAsync()
    {
        var sourceId = Data.SourceId;
        if (string.IsNullOrEmpty(sourceId))
        {
            return;
        }

        if (sourceId.StartsWith("http"))
        {
            await Launcher.LaunchUriAsync(new Uri(sourceId)).AsTask();
        }
    }
}
