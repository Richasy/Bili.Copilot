// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Popular;

/// <summary>
/// 流行视频页面控件基类.
/// </summary>
public abstract class PopularPageControlBase : LayoutUserControlBase<PopularPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularPageControlBase"/> class.
    /// </summary>
    protected PopularPageControlBase()
    {
        ViewModel = this.Get<PopularPageViewModel>();
    }
}
