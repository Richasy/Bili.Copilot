// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 启动页视图模型.
/// </summary>
public sealed partial class StartupPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPageViewModel"/> class.
    /// </summary>
    public StartupPageViewModel(
        IAuthenticationService authenticationService,
        ILogger<StartupPageViewModel> logger,
        DispatcherQueue dispatcherQueue)
    {
        _authenticationService = authenticationService;
        _logger = logger;
        _dispatcherQueue = dispatcherQueue;
    }

    /// <summary>
    /// 初始化视图模型.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task InitializeAsync(Image qrcodeImageControl)
    {
        Version = AppToolkit.GetPackageVersion();
        QRCodeImage = qrcodeImageControl;
        _cancellationTokenSource ??= new CancellationTokenSource();
        await _authenticationService.SignInAsync(cancellationToken: _cancellationTokenSource.Token).ConfigureAwait(true);
        var isSignedIn = await CheckAuthorizeStatusAsync().ConfigureAwait(true);
        if (isSignedIn)
        {
            ExitCommand.Execute(true);
        }
        else
        {
            _logger.LogWarning("未能成功获取授权信息，扫码可能出现了异常.");
        }
    }

    [RelayCommand]
    private async Task ExitAsync(bool shouldRestart)
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync().ConfigureAwait(true);
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        if (shouldRestart)
        {
            this.Get<AppViewModel>().RestartCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void RenderQRCode(byte[] imageData)
    {
        if (QRCodeImage is null)
        {
            throw new InvalidOperationException("二维码图片控件尚未就绪，请初始化模块.");
        }

        _dispatcherQueue.TryEnqueue(async () =>
        {
            using var stream = new MemoryStream(imageData);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream.AsRandomAccessStream()).AsTask().ConfigureAwait(true);
            QRCodeImage.Source = bitmap;
        });
    }

    private async Task<bool> CheckAuthorizeStatusAsync()
    {
        var tokenResolver = this.Get<IAuthenticationService>();
        try
        {
            await tokenResolver.EnsureTokenAsync(_cancellationTokenSource?.Token ?? default).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "检查授权状态时出现异常.");
        }

        return false;
    }
}
