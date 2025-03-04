// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.ViewLater;

/// <summary>
/// 稍后再看头部.
/// </summary>
public sealed partial class ViewLaterHeader : ViewLaterPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterHeader"/> class.
    /// </summary>
    public ViewLaterHeader() => InitializeComponent();

    private void OnCleanButtonClick(object sender, RoutedEventArgs e)
        => CleanTip.IsOpen = true;
}
