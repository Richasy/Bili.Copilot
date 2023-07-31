// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Forms;
using Bili.Copilot.Models.Constants.App;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 消息提醒.
/// </summary>
public sealed partial class TipPopup : UserControl
{
    /// <summary>
    /// <see cref="Text"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TipPopup), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    public TipPopup() => InitializeComponent();

    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    /// <param name="text">要显示的文本.</param>
    public TipPopup(string text)
        : this() => Text = text;

    /// <summary>
    /// 显示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 显示内容.
    /// </summary>
    /// <param name="type">信息级别.</param>
    /// <param name="displaySeconds">显示的时间.</param>
    public async void ShowAsync(InfoType type = InfoType.Information, double displaySeconds = 2)
    {
        PopupContainer.Status = type;
        await MainWindow.Instance.ShowTipAsync(this, displaySeconds);
    }
}
