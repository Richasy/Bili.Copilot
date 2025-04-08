// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 媒体 UI 提供器.
/// </summary>
public interface IMediaUIProvider
{
    /// <summary>
    /// 设置窗口视图模型.
    /// </summary>
    /// <param name="vm">视图模型.</param>
    public void SetWindowViewModel(MpvPlayerWindowViewModel vm);

    /// <summary>
    /// 提供用于显示的UI元素.
    /// </summary>
    /// <returns>元素.</returns>
    public UIElement GetUIElement();

    /// <summary>
    /// 显示错误信息.
    /// </summary>
    /// <param name="title">标题.</param>
    /// <param name="message">错误信息.</param>
    public void ShowError(string title, string message);
}
