// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 侧边底部加载控件.
/// </summary>
public sealed partial class SideBottomLoadingWidget : UserControl
{
    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(SideBottomLoadingWidget), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="SideBottomLoadingWidget"/> class.
    /// </summary>
    public SideBottomLoadingWidget() => InitializeComponent();

    /// <summary>
    /// 文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
