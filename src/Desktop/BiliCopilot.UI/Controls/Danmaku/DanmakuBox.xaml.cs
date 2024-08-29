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
}
