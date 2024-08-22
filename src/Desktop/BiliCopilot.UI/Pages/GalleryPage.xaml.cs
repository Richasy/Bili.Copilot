// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 图片库页面.
/// </summary>
public sealed partial class GalleryPage : GalleryPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryPage"/> class.
    /// </summary>
    public GalleryPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is (BiliImage image, IReadOnlyList<BiliImage> list))
        {
            ViewModel = new GalleryPageViewModel(image, list);
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        FactoryBlock.Text = 1.ToString("p00");
        ImageView.ScrollView.VerticalScrollMode = ScrollingScrollMode.Disabled;
        ImageView.ScrollView.HorizontalScrollMode = ScrollingScrollMode.Auto;
        ImageView.ScrollView.HorizontalScrollBarVisibility = ScrollingScrollBarVisibility.Auto;
        var index = ViewModel.Images.ToList().IndexOf(ViewModel.SelectedImage);
        ImageView.Select(index);
    }

    private void OnScrollViewerTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        => ViewModel.IsMenuHide = !ViewModel.IsMenuHide;

    private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (e.IsIntermediate)
        {
            FactoryBlock.Text = ImageScrollViewer.ZoomFactor.ToString("p00");
            CheckButtonStatus();
        }
    }

    private void CheckButtonStatus()
    {
        ZoomOutButton.IsEnabled = ImageScrollViewer.ZoomFactor > 0.2;
        ZoomInButton.IsEnabled = ImageScrollViewer.ZoomFactor < 1.5;
    }

    private void OnZoomInButtonClick(object sender, RoutedEventArgs e)
        => ImageScrollViewer.ChangeView(default, default, ImageScrollViewer.ZoomFactor + 0.1f);

    private void OnZoomOutButtonClick(object sender, RoutedEventArgs e)
        => ImageScrollViewer.ChangeView(default, default, ImageScrollViewer.ZoomFactor - 0.1f);

    private void OnRotateButtonClick(object sender, RoutedEventArgs e)
        => RotateTransform.Angle += 90;

    private void OnSettingToBackgroundClickAsync(object sender, RoutedEventArgs e)
        => ViewModel.SetWallPaperOrLockScreenCommand.Execute(true);

    private void OnSettingToLockScreenClickAsync(object sender, RoutedEventArgs e)
        => ViewModel.SetWallPaperOrLockScreenCommand.Execute(false);

    private void OnImageSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var image = sender.SelectedItem as BiliImage;
        ViewModel.ChangeImageCommand.Execute(image);
    }
}

/// <summary>
/// 图片库页面基类.
/// </summary>
public abstract class GalleryPageBase : LayoutPageBase<GalleryPageViewModel>
{
}
