// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Richasy.WinUI.Share.ViewModels;

namespace Richasy.WinUI.Share.Base;

/// <summary>
/// 用于布局的控件基类.
/// </summary>
public abstract class LayoutControlBase : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutControlBase"/> class.
    /// </summary>
    protected LayoutControlBase()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 控件绑定集合.
    /// </summary>
    protected virtual ControlBindings? ControlBindings { get; }

    /// <summary>
    /// 控件加载完成.
    /// </summary>
    protected virtual void OnControlLoaded()
    {
    }

    /// <summary>
    /// 控件卸载完成.
    /// </summary>
    protected virtual void OnControlUnloaded()
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        OnControlLoaded();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

        OnControlUnloaded();
    }
}

/// <summary>
/// 用于布局的控件基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public abstract class LayoutControlBase<TViewModel> : LayoutControlBase
    where TViewModel : ViewModelBase
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(LayoutControlBase), new PropertyMetadata(default));

    /// <summary>
    /// 视图模型.
    /// </summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
