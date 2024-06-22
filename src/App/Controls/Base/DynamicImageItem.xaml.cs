// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 图文动态条目.
/// </summary>
public sealed partial class DynamicImageItem : UserControl
{
    /// <summary>
    /// <see cref="ItemsSource"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DynamicImageItem), new PropertyMetadata(default, new PropertyChangedCallback(OnItemsSourceChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicImageItem"/> class.
    /// </summary>
    public DynamicImageItem()
        => InitializeComponent();

    /// <summary>
    /// 数据.
    /// </summary>
    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as DynamicImageItem;
        instance.LimitHeight();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => LimitHeight();

    private void OnLoaded(object sender, RoutedEventArgs e)
        => LimitHeight();

    private void OnImageTapped(object sender, TappedRoutedEventArgs e)
    {
        var image = sender as ImageEx;
        var sources = ItemsSource as List<Models.Data.Appearance.Image>;
        var index = sources.ToList().IndexOf(image.DataContext as Models.Data.Appearance.Image);
        AppViewModel.Instance.ShowImagesCommand.Execute(new Models.App.Args.ShowImageEventArgs(sources, index));
    }

    private void LimitHeight()
    {
        if (ItemsSource is List<Models.Data.Appearance.Image> images)
        {
            var imageCount = images.Count;
            var columnCount = ActualWidth / 100;
            var lineCount = Math.Ceiling(imageCount * 1.0 / columnCount);
            var height = (lineCount * 100) + ((lineCount - 1) * 4);
            if (ImageRepeater != null && !double.IsInfinity(height) && height >= 0)
            {
                ImageRepeater.Height = height;
            }
        }
    }
}
