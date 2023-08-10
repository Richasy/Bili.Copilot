// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播间条目的视图模型.
/// </summary>
public sealed partial class LiveItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveItemViewModel"/> class.
    /// </summary>
    /// <param name="information">直播间信息.</param>
    public LiveItemViewModel(LiveInformation information)
    {
        Data = information;
        ViewerCountText = NumberToolkit.GetCountText(information.ViewerCount);
    }

    [RelayCommand]
    private void Play()
        => AppViewModel.Instance.OpenPlayerCommand.Execute(new PlaySnapshot(Data.Identifier.Id, default, VideoType.Live)
        {
            Title = Data.Identifier.Title,
        });

    [RelayCommand]
    private async Task OpenInBrowserAsync()
    {
        var uri = $"https://live.bilibili.com/{Data.Identifier.Id}";
        _ = await Launcher.LaunchUriAsync(new Uri(uri));
    }

    [RelayCommand]
    private void Fix()
    {
        var cover = string.IsNullOrEmpty(Data.User?.Avatar.Uri) ? Data.Identifier.Cover.Uri : Data.User.Avatar.Uri;
        FixModuleViewModel.Instance.AddFixedItemCommand.Execute(new FixedItem(
            cover,
            Data.Identifier.Title,
            Data.Identifier.Id,
            FixedType.Live));
    }
}
