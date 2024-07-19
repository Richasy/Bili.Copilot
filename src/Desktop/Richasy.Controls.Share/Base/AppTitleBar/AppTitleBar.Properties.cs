// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Richasy.WinUI.Share.Base;

/// <summary>
/// 应用标题栏.
/// </summary>
public sealed partial class AppTitleBar
{
    /// <summary>
    /// <see cref="Header"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnHeaderChanged)));

    /// <summary>
    /// <see cref="IconElement"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IconElementProperty =
        DependencyProperty.Register(nameof(IconElement), typeof(IconElement), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnIconElementChanged)));

    /// <summary>
    /// <see cref="Title"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnTitleChanged)));

    /// <summary>
    /// <see cref="Subtitle"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnSubtitleChanged)));

    /// <summary>
    /// <see cref="Content"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnContentChanged)));

    /// <summary>
    /// <see cref="Footer"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(nameof(Footer), typeof(object), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnFooterChanged)));

    /// <summary>
    /// <see cref="IsBackButtonVisible"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsBackButtonVisibleProperty =
        DependencyProperty.Register(nameof(IsBackButtonVisible), typeof(bool), typeof(AppTitleBar), new PropertyMetadata(false, new PropertyChangedCallback(OnBackButtonVisibleChanged)));

    /// <summary>
    /// <see cref="IsBackEnabled"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsBackEnabledProperty =
        DependencyProperty.Register(nameof(IsBackButtonVisible), typeof(bool), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnIsBackEnabledChanged)));

    /// <summary>
    /// <see cref="IsPaneToggleButtonVisible"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsPaneToggleButtonVisibleProperty =
        DependencyProperty.Register(nameof(IsBackButtonVisible), typeof(bool), typeof(AppTitleBar), new PropertyMetadata(default, new PropertyChangedCallback(OnIsPaneToggleButtonVisibleChanged)));

    /// <summary>
    /// <see cref="TemplateSettings"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TemplateSettingsProperty =
        DependencyProperty.Register(nameof(TemplateSettings), typeof(AppTitleBarTemplateSettings), typeof(AppTitleBar), new PropertyMetadata(default));

    /// <summary>
    /// 头部内容.
    /// </summary>
    public object Header
    {
        get => (object)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// 图标资源.
    /// </summary>
    public IconElement IconElement
    {
        get => (IconElement)GetValue(IconElementProperty);
        set => SetValue(IconElementProperty, value);
    }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>
    /// 标题栏主体内容.
    /// </summary>
    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// 标题栏靠近标题按钮一侧的内容.
    /// </summary>
    public object Footer
    {
        get => (object)GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// 是否显示后退按钮.
    /// </summary>
    public bool IsBackButtonVisible
    {
        get => (bool)GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <summary>
    /// 后退行为是否可用.
    /// </summary>
    public bool IsBackEnabled
    {
        get => (bool)GetValue(IsBackEnabledProperty);
        set => SetValue(IsBackEnabledProperty, value);
    }

    /// <summary>
    /// 是否显示导航面板切换按钮.
    /// </summary>
    public bool IsPaneToggleButtonVisible
    {
        get => (bool)GetValue(IsPaneToggleButtonVisibleProperty);
        set => SetValue(IsPaneToggleButtonVisibleProperty, value);
    }

    /// <summary>
    /// 标题栏模板设置.
    /// </summary>
    public AppTitleBarTemplateSettings TemplateSettings
    {
        get => (AppTitleBarTemplateSettings)GetValue(TemplateSettingsProperty);
        set => SetValue(TemplateSettingsProperty, value);
    }
}
