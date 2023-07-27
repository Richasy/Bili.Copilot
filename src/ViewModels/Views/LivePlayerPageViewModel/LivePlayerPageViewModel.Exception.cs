// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播播放页视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    /// <summary>
    /// 显示错误信息.
    /// </summary>
    /// <param name="exception">错误信息.</param>
    public void DisplayException(Exception exception)
    {
        IsError = true;
        var msg = GetErrorMessage(exception);
        ErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestLiveFailed)}\n{msg}";
        LogException(exception);
    }
}
