// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.UI;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 直播弹幕项视图模型.
/// </summary>
public sealed partial class LiveDanmakuItemViewModel : ViewModelBase<LiveDanmakuInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveDanmakuItemViewModel"/> class.
    /// </summary>
    public LiveDanmakuItemViewModel(LiveDanmakuInformation data)
        : base(data)
    {
        Color = AppToolkit.HexToColor(data.UserLevelColor);
        Level = data.UserLevel;
        User = data.UserName;
        Message = data.Text;
    }

    /// <summary>
    /// 颜色.
    /// </summary>
    public Color Color { get; init; }

    /// <summary>
    /// 消息.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// 等级.
    /// </summary>
    public string Level { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    public string User { get; set; }
}
