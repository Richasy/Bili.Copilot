// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 可见性切换按钮.
/// </summary>
public sealed partial class VisibilityToggleButton : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Direction"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(VisibilityToggleButtonDirection), typeof(VisibilityToggleButton), new PropertyMetadata(VisibilityToggleButtonDirection.LeftToRightVisible));

    /// <summary>
    /// <see cref="IsHide"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty =
        DependencyProperty.Register(nameof(IsHide), typeof(bool), typeof(VisibilityToggleButton), new PropertyMetadata(default, new PropertyChangedCallback(OnIsHideChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="VisibilityToggleButton"/> class.
    /// </summary>
    public VisibilityToggleButton() => InitializeComponent();

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler Click;

    /// <summary>
    /// 获取或设置方向.
    /// </summary>
    public VisibilityToggleButtonDirection Direction
    {
        get => (VisibilityToggleButtonDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    /// <summary>
    /// 是否已经隐藏指定目标.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
        => ShowButton();

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerRoutedEventArgs e)
        => ShowButton();

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerRoutedEventArgs e)
        => HideButton();

    /// <inheritdoc/>
    protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        => HideButton();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => CheckButtonStates();

    private static void OnIsHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as VisibilityToggleButton;
        instance?.CheckButtonStates();
    }

    private void CheckButtonStates()
    {
        Icon.Symbol = IsHide
            ? Direction == VisibilityToggleButtonDirection.LeftToRightVisible ? FluentIcons.Common.Symbol.ChevronRight : FluentIcons.Common.Symbol.ChevronLeft
            : Direction == VisibilityToggleButtonDirection.LeftToRightVisible ? FluentIcons.Common.Symbol.ChevronLeft : FluentIcons.Common.Symbol.ChevronRight;
        var tip = IsHide ? ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Show) : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Hide);
        ToolTipService.SetToolTip(Btn, tip);
        AutomationProperties.SetName(Btn, tip);

        var container = VisualTreeHelper.GetParent(this) as FrameworkElement;
        if (container is not null)
        {
            container.Margin = IsHide
            ? Direction == VisibilityToggleButtonDirection.LeftToRightVisible ? new Thickness(-4, 0, 0, 0) : new Thickness(0, 0, -4, 0)
            : new Thickness(0);
        }
    }

    private void OnBtnClick(object sender, RoutedEventArgs e)
    {
        IsHide = !IsHide;
        Click?.Invoke(this, EventArgs.Empty);
    }

    private void ShowButton()
    {
        Btn.Visibility = Visibility.Visible;
        BackgroundGrid.Visibility = Visibility.Visible;
    }

    private void HideButton()
    {
        Btn.Visibility = Visibility.Collapsed;
        BackgroundGrid.Visibility = Visibility.Collapsed;
    }
}

/// <summary>
/// 可见性切换按钮方向.
/// </summary>
public enum VisibilityToggleButtonDirection
{
    /// <summary>
    /// 需要控制的内容在左侧.
    /// </summary>
    LeftToRightVisible,

    /// <summary>
    /// 需要控制的内容在右侧.
    /// </summary>
    RightToLeftVisible,
}
