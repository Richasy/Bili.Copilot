// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Controls;

/// <summary>
/// Prompt dialog when closing the window.
/// </summary>
public sealed partial class CloseWindowTipDialog : ContentDialog
{
    /// <summary>
    /// Dependency property for <see cref="IsNeverAskChecked"/>.
    /// </summary>
    public static readonly DependencyProperty IsNeverAskCheckedProperty =
        DependencyProperty.Register(nameof(IsNeverAskChecked), typeof(bool), typeof(CloseWindowTipDialog), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseWindowTipDialog"/> class.
    /// </summary>
    public CloseWindowTipDialog()
        => InitializeComponent();

    /// <summary>
    /// Is never ask checked.
    /// </summary>
    public bool IsNeverAskChecked
    {
        get => (bool)GetValue(IsNeverAskCheckedProperty);
        set => SetValue(IsNeverAskCheckedProperty, value);
    }
}
