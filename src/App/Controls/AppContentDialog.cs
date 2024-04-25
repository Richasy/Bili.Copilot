// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 应用内容对话框.
/// </summary>
public class AppContentDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppContentDialog"/> class.
    /// </summary>
    public AppContentDialog()
    {
        Opened += OnOpened;
        Closing += OnClosing;
    }

    private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        => AppViewModel.Instance.CurrentDialog = null;

    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        => AppViewModel.Instance.CurrentDialog = this;
}
