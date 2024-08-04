// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Users;

/// <summary>
/// 粉丝页面头部.
/// </summary>
public sealed partial class FansPageHeader : FansPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FansPageHeader"/> class.
    /// </summary>
    public FansPageHeader()
    {
        InitializeComponent();
    }

    internal static string CountText(int count) => count.ToString("N0");
}
