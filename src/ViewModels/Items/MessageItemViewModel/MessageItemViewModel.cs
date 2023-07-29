// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 消息条目视图模型.
/// </summary>
public sealed partial class MessageItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageItemViewModel"/> class.
    /// </summary>
    public MessageItemViewModel(MessageInformation information)
    {
        Data = information;
        PublishTime = information.PublishTime.Humanize();
    }

    [RelayCommand]
    private async Task ActiveAsync()
    {
        var sourceId = Data.SourceId;
        if (string.IsNullOrEmpty(sourceId))
        {
            return;
        }

        // 这表示应用暂时不处理此源，而是直接打开网页显示内容.
        if (sourceId.StartsWith("http"))
        {
            await Launcher.LaunchUriAsync(new Uri(sourceId));
        }
    }
}
