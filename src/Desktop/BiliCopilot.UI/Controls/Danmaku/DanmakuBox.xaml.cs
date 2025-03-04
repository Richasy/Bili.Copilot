// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Danmaku;

/// <summary>
/// 弹幕框.
/// </summary>
public sealed partial class DanmakuBox : DanmakuControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuBox"/> class.
    /// </summary>
    public DanmakuBox() => InitializeComponent();

    /// <summary>
    /// 输入框失去焦点.
    /// </summary>
    public event EventHandler InputLostFocus;

    /// <summary>
    /// 输入框获得焦点.
    /// </summary>
    public event EventHandler InputGotFocus;

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
        => InputLostFocus?.Invoke(this, EventArgs.Empty);

    private void OnInputFocus(object sender, RoutedEventArgs e)
        => InputGotFocus?.Invoke(this, EventArgs.Empty);
}
