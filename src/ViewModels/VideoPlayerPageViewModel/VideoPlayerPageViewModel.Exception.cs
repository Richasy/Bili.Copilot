// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频播放页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    /// <summary>
    /// 显示错误信息.
    /// </summary>
    public void DisplayException(Exception exception)
    {
        IsError = true;
        var msg = GetErrorMessage(exception);
        ErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFailed)}\n{msg}";
        LogException(exception);
    }

    private void DisplayFavoriteFoldersException(Exception exception)
    {
        IsFavoriteFoldersError = true;
        var msg = GetErrorMessage(exception);
        FavoriteFoldersErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFavoriteFailed)}\n{msg}";
        LogException(exception);
    }
}
