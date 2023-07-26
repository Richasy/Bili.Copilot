// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls;

/// <summary>
/// 错误面板，用于显示指定的错误内容.
/// </summary>
public sealed partial class ErrorPanel : UserControl
{
    /// <summary>
    /// <see cref="Text"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ErrorPanel), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="ActionContent"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(nameof(ActionContent), typeof(string), typeof(ErrorPanel), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="Command"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ErrorPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="SubActionContent"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SubActionContentProperty =
        DependencyProperty.Register(nameof(SubActionContent), typeof(string), typeof(ErrorPanel), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="SubCommand"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SubCommandProperty =
        DependencyProperty.Register(nameof(SubCommand), typeof(ICommand), typeof(ErrorPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorPanel"/> class.
    /// </summary>
    public ErrorPanel() => InitializeComponent();

    /// <summary>
    /// 错误文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 按钮文本.
    /// </summary>
    public string ActionContent
    {
        get => (string)GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    /// <summary>
    /// 命令.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// 次要按钮文本.
    /// </summary>
    public string SubActionContent
    {
        get => (string)GetValue(SubActionContentProperty);
        set => SetValue(SubActionContentProperty, value);
    }

    /// <summary>
    /// 次要命令.
    /// </summary>
    public ICommand SubCommand
    {
        get => (ICommand)GetValue(SubCommandProperty);
        set => SetValue(SubCommandProperty, value);
    }
}
