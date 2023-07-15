// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 卡片面板的字段.
/// </summary>
public partial class CardPanel
{
    private Grid _rootContainer;
    private long _pointerOverToken;
    private long _pressedToken;
}
