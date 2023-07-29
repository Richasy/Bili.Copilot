// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Danmaku;

/// <summary>
/// 弹幕显示选项.
/// </summary>
public sealed partial class DanmakuDisplayOptions : DanmakuDisplayOptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuDisplayOptions"/> class.
    /// </summary>
    public DanmakuDisplayOptions() => InitializeComponent();
}

/// <summary>
/// <see cref="DanmakuDisplayOptions"/> 的基类.
/// </summary>
public abstract class DanmakuDisplayOptionsBase : ReactiveUserControl<DanmakuModuleViewModel>
{
}
