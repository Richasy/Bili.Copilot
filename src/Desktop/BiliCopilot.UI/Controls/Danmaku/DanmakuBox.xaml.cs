// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 弹幕框.
/// </summary>
public sealed partial class DanmakuBox : DanmakuControlBase
{
    public static readonly DependencyProperty IsTextBoxVisibleProperty =
        DependencyProperty.Register(nameof(IsTextBoxVisible), typeof(bool), typeof(DanmakuBox), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuBox"/> class.
    /// </summary>
    public DanmakuBox() => InitializeComponent();

    public bool IsTextBoxVisible
    {
        get => (bool)GetValue(IsTextBoxVisibleProperty);
        set => SetValue(IsTextBoxVisibleProperty, value);
    }

    public bool IsTextBoxFocused { get; set; }

    private async void OnDanmakuInputBoxSubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (string.IsNullOrEmpty(args.QueryText))
        {
            return;
        }

        sender.IsEnabled = false;
        await ViewModel.SendDanmakuCommand.ExecuteAsync(args.QueryText);
        DispatcherQueue.TryEnqueue(() =>
        {
            sender.IsEnabled = true;
            sender.Text = string.Empty;
        });
    }

    private void OnDisplayFlyoutClosed(object sender, object e)
        => ViewModel.ResetStyle();

    private void OnInputLostFocus(object sender, RoutedEventArgs e)
        => IsTextBoxFocused = false;

    private void OnInputFocus(object sender, RoutedEventArgs e)
        => IsTextBoxFocused = true;
}
