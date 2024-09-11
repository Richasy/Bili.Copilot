// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.AI;

/// <summary>
/// AI 遮罩面板.
/// </summary>
public sealed partial class AIOverlayPanel : AIControlBase
{
    /// <summary>
    /// <see cref="IsPanelOpened"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsPanelOpenedProperty =
        DependencyProperty.Register(nameof(IsPanelOpened), typeof(bool), typeof(AIOverlayPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="AIOverlayPanel"/> class.
    /// </summary>
    public AIOverlayPanel() => InitializeComponent();

    /// <summary>
    /// 面板可见性.
    /// </summary>
    public bool IsPanelOpened
    {
        get => (bool)GetValue(IsPanelOpenedProperty);
        set => SetValue(IsPanelOpenedProperty, value);
    }

    private void OnHolderTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => IsPanelOpened = !IsPanelOpened;
}
