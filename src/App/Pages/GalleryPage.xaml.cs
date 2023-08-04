// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using WinRT.Interop;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 图片浏览页面.
/// </summary>
public sealed partial class GalleryPage : Page
{
    private readonly Dictionary<string, byte[]> _images;
    private int _currentIndex;
    private int _currentImageHeight;
    private bool _isControlShown;

    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryPage"/> class.
    /// </summary>
    public GalleryPage()
    {
        InitializeComponent();
        _images = new Dictionary<string, byte[]>();
        Images = new ObservableCollection<Models.Data.Appearance.Image>();
    }

    /// <summary>
    /// 图片地址.
    /// </summary>
    public ObservableCollection<Models.Data.Appearance.Image> Images { get; }

    /// <summary>
    /// 显示图片.
    /// </summary>
    /// <param name="index">图片索引.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowImageAsync(int index)
    {
        if (index >= 0 && Images.Count > index)
        {
            _currentIndex = index;
            if (ImageRepeater == null || !ImageRepeater.IsLoaded)
            {
                await Task.Delay(200);
            }

            SetSelectedItem(index);
            await LoadImageAsync(Images[index]);
        }
    }

    /// <inheritdoc/>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        var args = (ShowImageEventArgs)e.Parameter;
        _images.Clear();
        Images.Clear();
        args.Images.ToList().ForEach(Images.Add);
        FactoryBlock.Text = 1.ToString("p00");
        ShowControls();
        await ShowImageAsync(args.ShowIndex);
    }

    private async Task LoadImageAsync(Models.Data.Appearance.Image image)
    {
        _currentImageHeight = 0;
        RotateTransform.Angle = 0;
        ImageScrollViewer.ChangeView(null, null, 1f);

        if (Image.Source != null)
        {
            Image.Source = null;
        }

        var imageUrl = image.GetSourceUri().ToString();
        var hasCache = _images.TryGetValue(imageUrl, out var imageBytes);

        if (!hasCache)
        {
            using var client = new HttpClient();
            imageBytes = await client.GetByteArrayAsync(imageUrl);

            // 避免重复多次请求下插入同源数据.
            if (!_images.ContainsKey(imageUrl))
            {
                _images.Add(imageUrl, imageBytes);
            }
        }

        var bitmapImage = new BitmapImage();
        Image.Source = bitmapImage;
        var stream = new MemoryStream(imageBytes);
        await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
        _currentImageHeight = bitmapImage.PixelHeight;

        UpdateLayout();
        var factor = ImageScrollViewer.ViewportHeight / _currentImageHeight;
        if (factor > 1)
        {
            factor = 1;
        }

        ImageScrollViewer.ChangeView(null, null, (float)factor);
    }

    private void CheckButtonStatus()
    {
        ZoomOutButton.IsEnabled = ImageScrollViewer.ZoomFactor > 0.2;
        ZoomInButton.IsEnabled = ImageScrollViewer.ZoomFactor < 1.5;
    }

    private void SetSelectedItem(int index)
    {
        if (Images.Count <= 1)
        {
            return;
        }

        for (var i = 0; i < Images.Count; i++)
        {
            var element = ImageRepeater.GetOrCreateElement(i);
            if (element is CardPanel panel)
            {
                var image = panel.DataContext as Models.Data.Appearance.Image;
                panel.IsEnableCheck = true;
                panel.IsChecked = image == Images[index];
                panel.IsEnableCheck = false;
            }
        }
    }

    private void OnScrollViewerTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_isControlShown)
        {
            HideControls();
        }
        else
        {
            ShowControls();
        }
    }

    private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (e.IsIntermediate)
        {
            FactoryBlock.Text = ImageScrollViewer.ZoomFactor.ToString("p00");
            CheckButtonStatus();
        }
    }

    private void OnRotateButtonClick(object sender, RoutedEventArgs e)
        => RotateTransform.Angle += 90;

    private void OnZoomOutButtonClick(object sender, RoutedEventArgs e)
    {
        if (_currentImageHeight == 0)
        {
            return;
        }

        ImageScrollViewer.ChangeView(null, null, ImageScrollViewer.ZoomFactor - 0.1f);
    }

    private void OnZoomInButtonClick(object sender, RoutedEventArgs e)
    {
        if (_currentImageHeight == 0)
        {
            return;
        }

        ImageScrollViewer.ChangeView(null, null, ImageScrollViewer.ZoomFactor + 0.1f);
    }

    private async void OnImageItemClickAsync(object sender, RoutedEventArgs e)
    {
        var image = (sender as FrameworkElement).DataContext as Models.Data.Appearance.Image;
        var index = Images.IndexOf(image);
        await ShowImageAsync(index);
    }

    private async void OnNextButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (Images.Count - 1 <= _currentIndex)
        {
            return;
        }

        await ShowImageAsync(_currentIndex + 1);
    }

    private async void OnPrevButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (_currentIndex <= 0)
        {
            return;
        }

        await ShowImageAsync(_currentIndex - 1);
    }

    private void OnCopyButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (_currentIndex < 0 || _currentIndex > Images.Count - 1)
        {
            return;
        }

        var image = Images[_currentIndex];
        var dp = new DataPackage();
        dp.SetBitmap(RandomAccessStreamReference.CreateFromUri(image.GetSourceUri()));
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success);
    }

    private async void OnSaveButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (_currentIndex < 0 || _currentIndex > Images.Count - 1)
        {
            return;
        }

        var image = Images[_currentIndex];
        var imageUrl = image.GetSourceUri().ToString();
        var hasCache = _images.TryGetValue(imageUrl, out var cache);
        if (!hasCache)
        {
            return;
        }

        var savePicker = new FileSavePicker();
        var window = Frame.Tag as Window;
        var hwnd = WindowNative.GetWindowHandle(window);
        savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        InitializeWithWindow.Initialize(savePicker, hwnd);
        var fileName = Path.GetFileName(imageUrl);
        var extension = Path.GetExtension(imageUrl);
        savePicker.FileTypeChoices.Add($"{extension.TrimStart('.').ToUpper()} 图片", new string[] { extension });
        savePicker.SuggestedFileName = fileName;
        var file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            await FileIO.WriteBytesAsync(file, cache);
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.Saved), InfoType.Success);
        }
    }

    private async void OnSettingToBackgroundClickAsync(object sender, RoutedEventArgs e)
        => await SetWallpaperOrLockScreenAsync(true);

    private async void OnSettingToLockScreenClickAsync(object sender, RoutedEventArgs e)
        => await SetWallpaperOrLockScreenAsync(false);

    private async Task SetWallpaperOrLockScreenAsync(bool isWallpaper)
    {
        if (_currentIndex < 0 || _currentIndex > Images.Count - 1)
        {
            return;
        }

        var image = Images[_currentIndex];
        var imageUrl = image.GetSourceUri().ToString();
        var hasCache = _images.TryGetValue(imageUrl, out var cache);
        if (!hasCache)
        {
            return;
        }

        var profileSettings = UserProfilePersonalizationSettings.Current;
        var fileName = Path.GetFileName(imageUrl);
        var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteBytesAsync(file, cache);
        var result = isWallpaper
            ? await profileSettings.TrySetWallpaperImageAsync(file).AsTask()
            : await profileSettings.TrySetLockScreenImageAsync(file).AsTask();

        if (result)
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.SetSuccess), InfoType.Success);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.SetFailed), InfoType.Error);
        }

        await Task.Delay(1000);
        await file.DeleteAsync();
    }

    private void ShowControls()
    {
        TopContainer.Visibility = Visibility.Visible;
        ImageListContainer.Visibility = Images.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        _isControlShown = true;
    }

    private void HideControls()
    {
        TopContainer.Visibility = ImageListContainer.Visibility = Visibility.Collapsed;
        _isControlShown = false;
    }
}
