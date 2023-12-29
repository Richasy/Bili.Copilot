// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 加载覆盖层.
/// </summary>
public sealed partial class LoadingOverlapper : UserControl
{
    /// <summary>
    /// <see cref="IsOpen"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(LoadingOverlapper), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="Text"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(LoadingOverlapper), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadingOverlapper"/> class.
    /// </summary>
    public LoadingOverlapper() => InitializeComponent();

    /// <summary>
    /// 是否显示.
    /// </summary>
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// 提示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
