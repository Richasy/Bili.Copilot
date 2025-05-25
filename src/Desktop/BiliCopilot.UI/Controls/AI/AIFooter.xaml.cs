// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.System;
using Windows.UI.Core;

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

    protected override void OnControlUnloaded()
        => PromptRepeater.ItemsSource = null;

    private void OnMorePromptButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);

    private void OnInputBoxPreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);

            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.SendQuestionCommand.Execute(InputBox.Text);
                InputBox.Text = string.Empty;
            }
        }
    }

    private void OnQuickItemClick(object sender, RoutedEventArgs e)
        => MorePromptFlyout.Hide();
}
