// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Controls.Primitives;

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// AI 底部.
/// </summary>
public sealed partial class AIFooter : AIControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AIFooter"/> class.
    /// </summary>
    public AIFooter() => InitializeComponent();

    private void OnMorePromptButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
}
