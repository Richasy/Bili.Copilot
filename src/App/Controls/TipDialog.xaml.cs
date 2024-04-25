// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Toolkit;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// Tip dialog.
/// </summary>
public sealed partial class TipDialog : AppContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TipDialog"/> class.
    /// </summary>
    public TipDialog(string text)
    {
        InitializeComponent();
        TipBlock.Text = text;
        AppToolkit.ResetControlTheme(this);
    }
}
