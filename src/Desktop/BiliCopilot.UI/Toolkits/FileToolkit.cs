// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Windows.Storage;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Toolkits;

internal sealed class FileToolkit : SharedFileToolkit
{
    /// <summary>
    /// 本地数据是否存在.
    /// </summary>
    /// <returns>结果.</returns>
    public static bool IsLocalDataExist(string fileName, string folderName = "")
    {
        var folder = ApplicationData.GetDefault().LocalFolder;
        var path = string.IsNullOrEmpty(folderName)
            ? Path.Combine(folder.Path, fileName)
            : Path.Combine(folder.Path, folderName, fileName);
        return File.Exists(path);
    }
}
