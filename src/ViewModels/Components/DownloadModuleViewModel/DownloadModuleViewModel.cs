// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 下载模块视图模型.
/// </summary>
public sealed partial class DownloadModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadModuleViewModel"/> class.
    /// </summary>
    public DownloadModuleViewModel()
    {
        Parts = new ObservableCollection<VideoIdentifierSelectableViewModel>();
        OpenFolderWhenDownloaded = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.OpenFolderWhenDownloaded, true);
        AttachIsRunningToAsyncCommand(p => IsDownloading = p, DownloadCommand);
    }

    /// <summary>
    /// 设置下载数据.
    /// </summary>
    /// <param name="id">源 id.</param>
    /// <param name="parts">分集列表.</param>
    public void SetData(string id, IEnumerable<VideoIdentifier> parts = default)
    {
        _id = id;
        IsSupported = AppViewModel.Instance.IsDownloadSupported;
        TryClear(Parts);
        parts?.Select(p => new VideoIdentifierSelectableViewModel(p)).ToList().ForEach(Parts.Add);
        IsMultiPartShown = Parts.Count > 1;
        for (var i = 0; i < Parts.Count; i++)
        {
            Parts[i].Index = i + 1;
            Parts[i].IsSelected = true;
        }
    }

    [RelayCommand]
    private static async Task OpenConfigAsync()
    {
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/BBDown.config")).AsTask();
        await Windows.System.Launcher.LaunchFileAsync(file);
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        var sb = new StringBuilder();

        sb.Append(_id);
        var selectedParts = Parts.Where(p => p.IsSelected).Select(p => p.Index);
        if (selectedParts.Any())
        {
            sb.Append($" -p {string.Join(",", selectedParts)} ");
        }

        var token = await AuthorizeProvider.Instance.GetTokenAsync();
        sb.Append($" -token {token}");

        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "哔哩下载");
        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }

        sb.Append($" --work-dir \"{folderPath}\"");

        var packageFolder = Package.Current.InstalledPath;
        var configPath = Path.Combine(packageFolder, "Assets", "BBDown.config");
        sb.Append($" --config-file \"{configPath}\"");

        var process = new Process();
        process.StartInfo.FileName = "BBDown";
        process.StartInfo.Arguments = sb.ToString();
        process.StartInfo.UseShellExecute = true;

        process.Start();
        await process.WaitForExitAsync();
    }
}
