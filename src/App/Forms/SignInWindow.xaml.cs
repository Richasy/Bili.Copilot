// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Pages;
using Microsoft.UI.Windowing;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 欢迎窗口.
/// </summary>
public sealed partial class SignInWindow : WindowBase, ITipWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignInWindow"/> class.
    /// </summary>
    public SignInWindow()
    {
        InitializeComponent();
        IsMaximizable = false;
        IsMinimizable = false;
        IsResizable = false;

        AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

        Width = 700;
        Height = 460;

        this.CenterOnScreen();
        _ = MainFrame.Navigate(typeof(SignInPage));
    }

    /// <inheritdoc/>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        _ = TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }
}
