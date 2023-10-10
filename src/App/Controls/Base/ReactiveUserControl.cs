// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 反应式用户控件基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型类型.</typeparam>
public class ReactiveUserControl<TViewModel> : UserControl
    where TViewModel : class
{
    /// <summary>
    /// Dependency property for <see cref="ViewModel"/>.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(TViewModel), typeof(ReactiveUserControl<TViewModel>), new PropertyMetadata(default, new PropertyChangedCallback((dp, args) =>
            {
                var instance = dp as ReactiveUserControl<TViewModel>;
                instance.OnViewModelChanged(args);
            })));

    /// <summary>
    /// Dependency property for <see cref="CoreViewModel"/>.
    /// </summary>
    public static readonly DependencyProperty CoreViewModelProperty =
        DependencyProperty.Register(nameof(CoreViewModel), typeof(AppViewModel), typeof(ReactiveUserControl<TViewModel>), new PropertyMetadata(AppViewModel.Instance));

    /// <summary>
    /// 视图模型.
    /// </summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// 核心视图模型.
    /// </summary>
    public AppViewModel CoreViewModel
    {
        get => (AppViewModel)GetValue(CoreViewModelProperty);
        set => SetValue(CoreViewModelProperty, value);
    }

    /// <summary>
    /// 当 <see cref="ViewModel"/> 改变时调用，可重写此方法以实现自定义逻辑.
    /// </summary>
    /// <param name="e">Dependency properties change event arguments.</param>
    internal virtual void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        // Do nothing.
    }
}
