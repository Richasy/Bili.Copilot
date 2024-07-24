// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 启动页视图模型.
/// </summary>
public sealed partial class StartupPageViewModel : ViewModelBase
{
    /// <summary>
    /// 初始化视图模型.
    /// </summary>
    public void Initialize(Image qrcodeImageControl)
    {
        QRCodeImage = qrcodeImageControl;
    }

    [RelayCommand]
    private async Task RenderQRCodeAsync(byte[] imageData)
    {
        if (QRCodeImage is null)
        {
            throw new InvalidOperationException("二维码图片控件尚未就绪，请初始化模块.");
        }

        using var stream = new MemoryStream(imageData);
        var bitmap = new BitmapImage();
        await bitmap.SetSourceAsync(stream.AsRandomAccessStream()).AsTask().ConfigureAwait(true);
        QRCodeImage.Source = bitmap;
    }
}
