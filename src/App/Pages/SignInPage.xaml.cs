// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 登录界面.
/// </summary>
public sealed partial class SignInPage : PageBase
{
    private readonly AuthorizeProvider _authorizeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignInPage"/> class.
    /// </summary>
    public SignInPage()
    {
        _authorizeProvider = AuthorizeProvider.Instance;
        InitializeComponent();
        VersionBlock.Text = AppToolkit.GetPackageVersion();
        CoreViewModel.BackRequest += OnBackRequest;
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        _authorizeProvider.QRCodeStatusChanged += OnQRCodeStatusChanged;
        await LoadQRCodeAsync();
    }

    private void OnBackRequest(object sender, EventArgs e)
        => WebContainer.Visibility = Visibility.Collapsed;

    private void OnQRCodeStatusChanged(object sender, Tuple<QRCodeStatus, TokenInfo> e)
    {
        switch (e.Item1)
        {
            case QRCodeStatus.Expired:
                ShowQRTip(StringNames.QRCodeExpired);
                _authorizeProvider.StopQRLoginListener();
                break;
            case QRCodeStatus.Success:
                _authorizeProvider.StopQRLoginListener();
                TraceLogger.LogSignIn();
                CoreViewModel.RestartCommand.Execute(default);
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

    private void OnDataUsageButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private void OnWebSignInButtonClick(object sender, RoutedEventArgs e)
    {
        WebContainer.Visibility = Visibility.Visible;
        if (OverlayFrame.Content is null)
        {
            OverlayFrame.Navigate(typeof(WebSignInPage));
        }
    }
}
