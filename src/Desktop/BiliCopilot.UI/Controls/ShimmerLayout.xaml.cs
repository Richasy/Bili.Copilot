// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 骨架占位图.
/// </summary>
public sealed partial class ShimmerLayout : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsActive"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ShimmerLayout), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ShimmerLayout"/> class.
    /// </summary>
    public ShimmerLayout() => InitializeComponent();

    /// <summary>
    /// 是否激活.
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? default : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);
}
