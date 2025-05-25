// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 动态图片画廊.
/// </summary>
public sealed partial class MomentImageGallery : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="ItemsSource"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(MomentImageGallery), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="MomentImageGallery"/> class.
    /// </summary>
    public MomentImageGallery() => InitializeComponent();

    /// <summary>
    /// 图片源.
    /// </summary>
    public object ItemsSource
    {
        get => (object)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void OnControlUnloaded()
        => ImageRepeater.ItemsSource = default;

    private void OnImageTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        e.Handled = true;
        var image = (sender as Image).Tag as BiliImage;
        var itemsSource = ItemsSource as List<BiliImage>;
        new GalleryWindow(image, itemsSource).Activate();
    }
}
