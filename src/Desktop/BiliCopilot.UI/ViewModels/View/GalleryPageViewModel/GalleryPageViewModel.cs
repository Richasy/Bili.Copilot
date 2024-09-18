// Copyright (c) Bili Copilot. All rights reserved.

using System.Net.Http;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.WinUI.Share.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.UserProfile;
using WinRT.Interop;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 图片库页面视图模型.
/// </summary>
public sealed partial class GalleryPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GalleryPageViewModel"/> class.
    /// </summary>
    public GalleryPageViewModel(
        BiliImage image,
        List<BiliImage>? images = default)
    {
        SelectedImage = image;
        Images = images ?? new List<BiliImage> { image };
        IsGroup = Images?.Count > 1;
        IsMenuHide = false;
    }

    [RelayCommand]
    private void ChangeImage(BiliImage image)
    {
        if (image == null || SelectedImage == image)
        {
            return;
        }

        SelectedImage = image;
    }

    [RelayCommand]
    private void NextImage()
    {
        var index = Images.ToList().IndexOf(SelectedImage);
        if (index == Images.Count - 1)
        {
            return;
        }

        index++;
        ChangeImage(Images.ElementAt(index));
    }

    [RelayCommand]
    private void PreviousImage()
    {
        var index = Images.ToList().IndexOf(SelectedImage);
        if (index == 0)
        {
            return;
        }

        index--;
        ChangeImage(Images.ElementAt(index));
    }

    [RelayCommand]
    private void CopyImage()
    {
        if (SelectedImage is null)
        {
            return;
        }

        var dp = new DataPackage();
        dp.SetWebLink(SelectedImage.SourceUri);
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private async Task SaveImageAsync()
    {
        if (SelectedImage is null)
        {
            return;
        }

        var fileName = Path.GetFileName(SelectedImage.SourceUri.ToString());
        var extension = Path.GetExtension(fileName);
        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            SuggestedFileName = fileName,
        };

        savePicker.FileTypeChoices.Add($"{extension.TrimStart('.').ToUpper()} 图片", [extension]);
        InitializeWithWindow.Initialize(savePicker, this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle());
        var file = await savePicker.PickSaveFileAsync();
        if (file is null)
        {
            return;
        }

        using var stream = await file.OpenStreamForWriteAsync();
        using var client = new HttpClient();
        using var response = await client.GetAsync(SelectedImage.SourceUri);
        await response.Content.CopyToAsync(stream);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Saved), InfoType.Success));
    }

    [RelayCommand]
    private async Task SetWallPaperOrLockScreenAsync(bool isWallPaper)
    {
        if (SelectedImage is null)
        {
            return;
        }

        var profileSettings = UserProfilePersonalizationSettings.Current;
        var fileName = Path.GetFileName(SelectedImage.SourceUri.ToString());
        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("TempImages", CreationCollisionOption.OpenIfExists);
        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        using var stream = await file.OpenStreamForWriteAsync();
        using var client = new HttpClient();
        using var response = await client.GetAsync(SelectedImage.SourceUri);
        await response.Content.CopyToAsync(stream);
        var result = isWallPaper
            ? await profileSettings.TrySetWallpaperImageAsync(file)
            : await profileSettings.TrySetLockScreenImageAsync(file);
        if (result)
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.SetSuccess), InfoType.Success));
        }
        else
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.SetFailed), InfoType.Error));
        }
    }
}
