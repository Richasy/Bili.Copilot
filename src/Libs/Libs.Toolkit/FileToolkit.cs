// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Bili.Copilot.Libs.Toolkit;

/// <summary>
/// File Toolkit.
/// </summary>
public static class FileToolkit
{
    /// <summary>
    /// Get local data and convert.
    /// </summary>
    /// <typeparam name="T">Conversion target type.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="defaultValue">The default value when the file does not exist or has no content.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns>Converted result.</returns>
    public static Task<T> ReadLocalDataAsync<T>(string fileName, string defaultValue = "{}", string folderName = "") => Task.Run(async () =>
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

        return typeof(T) == typeof(string) ? (T)content.Clone() : JsonSerializer.Deserialize<T>(content);
    });

    /// <summary>
    /// Write data to local file.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="data">Data to be written.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static Task WriteLocalDataAsync<T>(string fileName, T data, string folderName = "") => Task.Run(async () =>
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists)
                        .AsTask();
        }

        var writeContent = string.Empty;
        writeContent = data is string ? data.ToString() : JsonSerializer.Serialize(data);

        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists)
                    .AsTask();

        await FileIO.WriteTextAsync(file, writeContent)
          .AsTask();
    });

    /// <summary>
    /// Delete local data file.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static Task DeleteLocalDataAsync(string fileName, string folderName = "") => Task.Run(async () =>
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
    });

    /// <summary>
    /// Open the file chooser and read the selected file.
    /// </summary>
    /// <param name="types">Allowed file extension.</param>
    /// <returns><c>Item1</c> represents the content of the file, <c>Item2</c> means file extension name.</returns>
    public static async Task<Tuple<string, string>> OpenLocalFileAndReadAsync(params string[] types)
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        var typeReg = new Regex(@"^\.[a-zA-Z0-9]+$");
        foreach (var type in types)
        {
            if (type == "*" || typeReg.IsMatch(type))
            {
                picker.FileTypeFilter.Add(type);
            }
            else
            {
                throw new InvalidCastException("Invalid file extension.");
            }
        }

        var file = await picker.PickSingleFileAsync();

        if (file != null)
        {
            var content = await FileIO.ReadTextAsync(file);
            return new Tuple<string, string>(content, file.FileType);
        }

        return null;
    }

    /// <summary>
    /// Open file which in the package.
    /// </summary>
    /// <param name="filePath">Start with ms-appx:///.</param>
    /// <returns>Text.</returns>
    public static async Task<string> ReadPackageFileAsync(string filePath)
    {
        try
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));
            return await FileIO.ReadTextAsync(file);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

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
        var file = await picker.PickSingleFileAsync().AsTask();
        return file;
    }
}
