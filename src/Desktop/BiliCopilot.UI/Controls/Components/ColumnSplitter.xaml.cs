// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 分割器.
/// </summary>
public sealed partial class ColumnSplitter : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="IsHide"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideProperty =
        DependencyProperty.Register(nameof(IsHide), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="ColumnWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ColumnWidthProperty =
        DependencyProperty.Register(nameof(ColumnWidth), typeof(double), typeof(ColumnSplitter), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="MinColumnWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MinColumnWidthProperty =
        DependencyProperty.Register(nameof(MinColumnWidth), typeof(double), typeof(ColumnSplitter), new PropertyMetadata(220d));

    /// <summary>
    /// <see cref="MaxColumnWidth"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty MaxColumnWidthProperty =
        DependencyProperty.Register(nameof(MaxColumnWidth), typeof(double), typeof(ColumnSplitter), new PropertyMetadata(300d));

    /// <summary>
    /// <see cref="IsHideButtonEnabled"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsHideButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsHideButtonEnabled), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(true));

    /// <summary>
    /// <see cref="IsInvertDirection"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsInvertDirectionProperty =
        DependencyProperty.Register(nameof(IsInvertDirection), typeof(bool), typeof(ColumnSplitter), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnSplitter"/> class.
    /// </summary>
    public ColumnSplitter() => InitializeComponent();

    /// <summary>
    /// 是否进入隐藏状态.
    /// </summary>
    public bool IsHide
    {
        get => (bool)GetValue(IsHideProperty);
        set => SetValue(IsHideProperty, value);
    }

    /// <summary>
    /// 列宽度.
    /// </summary>
    public double ColumnWidth
    {
        get => (double)GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }

    /// <summary>
    /// 最小列宽度.
    /// </summary>
    public double MinColumnWidth
    {
        get => (double)GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    /// <summary>
    /// 最大列宽度.
    /// </summary>
    public double MaxColumnWidth
    {
        get => (double)GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }

    /// <summary>
    /// 是否显示隐藏按钮.
    /// </summary>
    public bool IsHideButtonEnabled
    {
        get => (bool)GetValue(IsHideButtonEnabledProperty);
        set => SetValue(IsHideButtonEnabledProperty, value);
    }

    /// <summary>
    /// 是否反转方向. 默认是左向右.
    /// </summary>
    public bool IsInvertDirection
    {
        get => (bool)GetValue(IsInvertDirectionProperty);
        set => SetValue(IsInvertDirectionProperty, value);
    }

    /// <inheritdoc/>
    protected override ControlBindings ControlBindings => Bindings is null ? default : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (IsInvertDirection)
        {
            Sizer.IsDragInverted = true;
            ToggleBtn.Direction = VisibilityToggleButtonDirection.RightToLeftVisible;
            ToggleBtn.Margin = new Thickness(-32, 0, 0, 0);
            ToggleBtn.CornerRadius = new CornerRadius(6, 0, 0, 6);
        }
    }
}
