// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Users;

/// <summary>
/// 粉丝页面头部.
/// </summary>
public sealed partial class FansHeader : FansPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansHeader"/> class.
    /// </summary>
    public FansHeader() => InitializeComponent();

    internal static string CountText(int count) => count.ToString("N0");
}
