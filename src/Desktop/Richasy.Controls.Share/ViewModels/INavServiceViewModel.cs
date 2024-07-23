// Copyright (c) Bili Copilot. All rights reserved.

namespace Richasy.WinUI.Share.ViewModels;

/// <summary>
/// 导航服务视图模型.
/// </summary>
public interface INavServiceViewModel
{
    /// <summary>
    /// 导航到指定页面.
    /// </summary>
    void NavigateTo(string pageKey, object? parameter = default);
}
