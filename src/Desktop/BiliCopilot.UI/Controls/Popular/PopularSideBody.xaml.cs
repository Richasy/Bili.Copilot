// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Popular;

/// <summary>
/// 流行视频页面侧边栏主体.
/// </summary>
public sealed partial class PopularSideBody : PopularPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopularSideBody"/> class.
    /// </summary>
    public PopularSideBody()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var context = e.AddedItems.FirstOrDefault();
        if (context != null)
        {
            // Selected.
        }
    }
}
