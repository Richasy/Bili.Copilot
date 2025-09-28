// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 在非当前页时需要取消加载的页面视图模型接口。
/// </summary>
public interface ICancelPageViewModel
{
    public void CancelLoading();
}
