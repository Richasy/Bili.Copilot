// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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

    /// <summary>
    /// Get local data and convert.
    /// </summary>
    /// <typeparam name="T">Conversion target type.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="typeInfo">Json type info for deserialize.</param>
    /// <param name="defaultValue">The default value when the file does not exist or has no content.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns>Converted result.</returns>
    public static async Task<T> ReadLocalDataAsync<T>(string fileName, JsonTypeInfo<T> typeInfo, string defaultValue = "{}", string folderName = "")
    {
        var path = string.IsNullOrEmpty(folderName) ?
                        $"ms-appdata:///local/{fileName}" :
                        $"ms-appdata:///local/{folderName}/{fileName}";
        var content = defaultValue;
        try
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path))
                    .AsTask();
            var fileContent = await FileIO.ReadTextAsync(file)
                           .AsTask();

            if (!string.IsNullOrEmpty(fileContent))
            {
                content = fileContent;
            }
        }
        catch (FileNotFoundException)
        {
        }

        return typeof(T) == typeof(string) ? (T)content.Clone() : JsonSerializer.Deserialize<T>(content, typeInfo);
    }

    /// <summary>
    /// Write data to local file.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="data">Data to be written.</param>
    /// <param name="typeInfo">Type info for serialize.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task WriteLocalDataAsync<T>(string fileName, T data, JsonTypeInfo<T> typeInfo, string folderName = "")
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists)
                        .AsTask();
        }

        var writeContent = string.Empty;
        writeContent = data is string ? data.ToString() : JsonSerializer.Serialize(data, typeInfo);

        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists)
                    .AsTask();

        await FileIO.WriteTextAsync(file, writeContent)
          .AsTask();
    }

    /// <summary>
    /// Delete local data file.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task DeleteLocalDataAsync(string fileName, string folderName = "")
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName)
                        .AsTask();
        }

        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists)
                    .AsTask();
        await file.DeleteAsync()
            .AsTask();
    }
}
