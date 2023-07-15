// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 应用页面基类.
/// </summary>
public class PageBase : Page
{
    /// <summary>
    /// <see cref="CoreViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CoreViewModelProperty =
        DependencyProperty.Register(nameof(CoreViewModel), typeof(AppViewModel), typeof(PageBase), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="PageBase"/> class.
    /// </summary>
    public PageBase()
    {
        CoreViewModel = AppViewModel.Instance;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 应用核心视图模型.
    /// </summary>
    public AppViewModel CoreViewModel
    {
        get => (AppViewModel)GetValue(CoreViewModelProperty);
        set => SetValue(CoreViewModelProperty, value);
    }

    /// <summary>
    /// 获取页面注册的视图模型.
    /// </summary>
    /// <returns>视图模型，如果没有则返回<c>null</c>.</returns>
    public virtual object GetViewModel() => null;

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        GC.Collect();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 在页面加载完成后调用.
    /// </summary>
    protected virtual void OnPageLoaded()
    {
    }

    /// <summary>
    /// 在页面卸载完成后调用.
    /// </summary>
    protected virtual void OnPageUnloaded()
    {
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OnPageLoaded();

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => OnPageUnloaded();
}

/// <summary>
/// 带视图模型的应用页面基类.
/// </summary>
/// <typeparam name="TViewModel">视图模型.</typeparam>
public class PageBase<TViewModel> : PageBase
    where TViewModel : class
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(PageBase), new PropertyMetadata(default));

    /// <summary>
    /// 页面的视图模型.
    /// </summary>
    public TViewModel ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    public override object GetViewModel() => ViewModel;
}
