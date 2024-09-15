// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
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
using Windows.System;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 直播条目视图模型.
/// </summary>
public sealed partial class LiveItemViewModel : ViewModelBase<LiveInformation>
{
    private readonly Action<LiveItemViewModel>? _removeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveItemViewModel"/> class.
    /// </summary>
    public LiveItemViewModel(LiveInformation data, Action<LiveItemViewModel>? removeAction = default)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover?.Uri;
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

        _removeAction = removeAction;
    }

    [RelayCommand]
    private void Play()
    {
        var preferDisplayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        if (preferDisplayMode == PlayerDisplayMode.NewWindow)
        {
            OpenInNewWindowCommand.Execute(default);
            return;
        }

        var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
        if (preferPlayer == PlayerType.Web)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(WebPlayerPage).FullName, GetWebUrl());
            return;
        }

        this.Get<NavigationViewModel>().NavigateToOver(typeof(LivePlayerPage).FullName, Data.Identifier);
    }

    [RelayCommand]
    private Task OpenInBroswerAsync()
        => Launcher.LaunchUriAsync(new Uri(GetWebUrl())).AsTask();

    [RelayCommand]
    private void OpenInNewWindow()
        => new PlayerWindow().OpenLive(Data.Identifier);

    [RelayCommand]
    private async Task RemoveHistoryAsync()
    {
        try
        {
            await this.Get<IViewHistoryService>().RemoveLiveHistoryItemAsync(Data);
            _removeAction?.Invoke(this);
        }
        catch (Exception ex)
        {
            this.Get<ILogger<LiveItemViewModel>>().LogError(ex, "移除直播历史记录失败.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.FailedToRemoveVideoFromHistory), InfoType.Error));
        }
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage).FullName, Data.User);

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem(Data.Identifier.Id, Data.Identifier.Title, Data.Identifier.Cover.Uri.ToString(), PinContentType.Live);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private string GetWebUrl()
        => $"https://live.bilibili.com/{Data.Identifier.Id}";
}
