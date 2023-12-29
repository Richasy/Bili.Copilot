// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Danmaku;

/// <summary>
/// 弹幕发送选项.
/// </summary>
public sealed partial class DanmakuSendOptions : DanmakuSendOptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuSendOptions"/> class.
    /// </summary>
    public DanmakuSendOptions() => InitializeComponent();

    private void OnColorItemClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as FrameworkElement).DataContext as KeyValue<string>;
        ViewModel.Color = item.Value;
    }

    private void OnSizeItemClick(object sender, RoutedEventArgs e)
        => ViewModel.IsStandardSize = !ViewModel.IsStandardSize;
}

/// <summary>
/// <see cref="DanmakuSendOptions"/> 的基类.
/// </summary>
public abstract class DanmakuSendOptionsBase : ReactiveUserControl<DanmakuModuleViewModel>
{
}
