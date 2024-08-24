﻿// Copyright (c) Bili Copilot. All rights reserved.

using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace BiliCopilot.UI.Toolkits;

internal static class FileToolkit
{
    /// <summary>
    /// 选择文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件对象（可能为空）.</returns>
    public static async Task<StorageFile> PickFileAsync(string extension, object windowInstance)
    {
        var picker = new FileOpenPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
        picker.FileTypeFilter.Add(extension);
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        return await picker.PickSingleFileAsync().AsTask();
    }

    /// <summary>
    /// 选择文件夹.
    /// </summary>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件夹.</returns>
    public static async Task<StorageFolder> PickFolderAsync(object windowInstance)
    {
        var picker = new FolderPicker();
        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add("*");
        var folder = await picker.PickSingleFolderAsync().AsTask();
        return folder;
    }
}
