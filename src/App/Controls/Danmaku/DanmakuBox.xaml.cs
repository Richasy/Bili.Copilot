// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Danmaku;

/// <summary>
/// 弹幕消息框.
/// </summary>
public sealed partial class DanmakuBox : DanmakuBoxBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuBox"/> class.
    /// </summary>
    public DanmakuBox() => InitializeComponent();

    /// <summary>
    /// 是否正在聚焦输入框.
    /// </summary>
    public bool IsInputFocused { get; private set; }

    private async void OnDanmakuInputBoxSubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.QueryText))
        {
            sender.IsEnabled = false;
            await ViewModel.SendDanmakuCommand.ExecuteAsync(args.QueryText);
            _ = DispatcherQueue.TryEnqueue(() =>
            {
                sender.IsEnabled = true;
                sender.Text = string.Empty;
            });
        }
    }

    private void OnDanmakuInputBoxGotFocus(object sender, RoutedEventArgs e)
        => IsInputFocused = true;

    private void OnDanmakuInputBoxLostFocus(object sender, RoutedEventArgs e)
        => IsInputFocused = false;
}

/// <summary>
/// <see cref="DanmakuBox"/> 的基类.
/// </summary>
public abstract class DanmakuBoxBase : ReactiveUserControl<DanmakuModuleViewModel>
{
}
