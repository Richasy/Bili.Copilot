// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 欢迎界面（x）登录界面（√）.
/// </summary>
public sealed partial class SignInPage : Page
{
    private readonly AuthorizeProvider _authorizeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignInPage"/> class.
    /// </summary>
    public SignInPage()
    {
        _authorizeProvider = AuthorizeProvider.Instance;
        InitializeComponent();
        Loaded += OnLoadedAsync;
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        _authorizeProvider.QRCodeStatusChanged += OnQRCodeStatusChangedAsync;
        await LoadQRCodeAsync();
    }

    private async void OnQRCodeStatusChangedAsync(object sender, Tuple<QRCodeStatus, TokenInfo> e)
    {
        switch (e.Item1)
        {
            case QRCodeStatus.Expired:
                ShowQRTip(StringNames.QRCodeExpired);
                _authorizeProvider.StopQRLoginListener();
                break;
            case QRCodeStatus.Success:
                _authorizeProvider.StopQRLoginListener();
                await AppViewModel.Instance.InitializeAsync();
                break;
            case QRCodeStatus.Failed:
                ShowQRTip(StringNames.LoginFailed);
                _authorizeProvider.StopQRLoginListener();
                break;
            default:
                break;
        }
    }

    private async Task LoadQRCodeAsync()
    {
        HideQRTip();
        QRLoadingShimmer.Visibility = Visibility.Visible;
        RefreshQRButton.Visibility = Visibility.Collapsed;
        QRTipBlock.Visibility = Visibility.Collapsed;
        var stream = await _authorizeProvider.GetQRImageAsync();
        if (stream != null)
        {
            using var ms = (MemoryStream)stream;
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(ms.AsRandomAccessStream());
            QRCodeImage.Source = bitmapImage;
            _authorizeProvider.StartQRLoginListener();
            QRTipBlock.Visibility = Visibility.Visible;
        }
        else
        {
            ShowQRTip(StringNames.FailedToLoadQRCode);
        }

        RefreshQRButton.Visibility = Visibility.Visible;
        QRLoadingShimmer.Visibility = Visibility.Collapsed;
    }

    private void ShowQRTip(StringNames name)
    {
        var msg = ResourceToolkit.GetLocalizedString(name);
        QRMaskContainer.Visibility = Visibility.Visible;
        QRTipBlock.Text = msg;
    }

    private void HideQRTip()
    {
        QRMaskContainer.Visibility = Visibility.Collapsed;
        QRTipBlock.Text = string.Empty;
    }

    private async void OnRefreshQRButtonClickAsync(object sender, RoutedEventArgs e)
        => await LoadQRCodeAsync();

    private async void OnRepoButtonClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("https://github.com/Richasy/Bili.Copilot"));

    private async void OnBiliButtonClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("https://space.bilibili.com/5992670"));
}
