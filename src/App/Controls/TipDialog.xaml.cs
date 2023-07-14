// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// Tip dialog.
/// </summary>
public sealed partial class TipDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TipDialog"/> class.
    /// </summary>
    public TipDialog(string text)
    {
        InitializeComponent();
        TipBlock.Text = text;
    }
}
