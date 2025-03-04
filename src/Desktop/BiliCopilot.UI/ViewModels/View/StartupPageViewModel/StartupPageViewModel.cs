// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;

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
        Version = this.Get<IAppToolkit>().GetPackageVersion();
        QRCodeImage = qrcodeImageControl;
        await ReloadQRCodeAsync();
    }

    [RelayCommand]
    private async Task ReloadQRCodeAsync()
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            IsQRCodeLoading = true;
            await _authenticationService.SignInAsync(cancellationToken: _cancellationTokenSource.Token);
            var isSignedIn = await CheckAuthorizeStatusAsync();
            if (isSignedIn)
            {
                ExitCommand.Execute(true);
            }
            else
            {
                _logger.LogWarning("未能成功获取授权信息，扫码可能出现了异常.");
            }
        }
        catch (Exception ex)
        {
            IsQRCodeLoading = false;
            ErrorTip = ex.Message;
            _logger.LogInformation(ex, "登录过程中出现异常.");
        }
    }

    [RelayCommand]
    private async Task ExitAsync(bool shouldRestart)
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
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
            await bitmap.SetSourceAsync(stream.AsRandomAccessStream()).AsTask();
            QRCodeImage.Source = bitmap;
            IsQRCodeLoading = false;
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
