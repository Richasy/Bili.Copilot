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
using Microsoft.UI.Xaml;
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
    public DownloadModuleViewModel(Window attachedWindow)
    {
        _attachedWindow = attachedWindow;
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
        _configPath = SettingsToolkit.ReadLocalSetting(Models.Constants.App.SettingNames.BBDownConfigPath, string.Empty);
        IsBBDownConfigLinked = !string.IsNullOrEmpty(_configPath) && File.Exists(_configPath);
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
    private async Task OpenConfigAsync()
    {
        if (!File.Exists(_configPath))
        {
            return;
        }

        var file = await StorageFile.GetFileFromPathAsync(_configPath).AsTask();
        await Windows.System.Launcher.LaunchFileAsync(file);
    }

    [RelayCommand]
    private async Task CreateDefaultConfigAsync()
    {
        var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var configPath = Path.Combine(docFolder, "BBDown.config");
        if (!File.Exists(configPath))
        {
            var stream = File.Create(configPath);
            stream.Dispose();
        }

        await File.WriteAllTextAsync(configPath, "-mt");
        _configPath = configPath;
        IsBBDownConfigLinked = true;
        SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.BBDownConfigPath, _configPath);
    }

    [RelayCommand]
    private async Task LinkConfigFileAsync()
    {
        var file = await FileToolkit.PickFileAsync(".config", _attachedWindow);
        if (file != null)
        {
            _configPath = file.Path;
            SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.BBDownConfigPath, _configPath);
            IsBBDownConfigLinked = true;
        }
    }

    [RelayCommand]
    private void ResetConfig()
    {
        _configPath = string.Empty;
        SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.BBDownConfigPath, string.Empty);
        IsBBDownConfigLinked = false;
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        var sb = new StringBuilder();
        sb.Append(_id);

        var token = await AuthorizeProvider.Instance.GetTokenAsync();
        sb.Append($" -token {token}");

        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "哔哩下载");
        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }

        sb.Append($" --work-dir \"{folderPath}\"");

        if (!string.IsNullOrEmpty(_configPath) && File.Exists(_configPath))
        {
            var configs = await File.ReadAllLinesAsync(_configPath);
            var hasPartParams = configs.Any(c => c.StartsWith("-p") || c.StartsWith("--select-page"));
            if (!hasPartParams)
            {
                var selectedParts = Parts.Where(p => p.IsSelected).Select(p => p.Index);
                if (selectedParts.Any())
                {
                    sb.Append($" -p {string.Join(",", selectedParts)} ");
                }
            }

            sb.Append($" --config-file \"{_configPath}\"");
        }

        var process = new Process();
        process.StartInfo.FileName = "BBDown";
        process.StartInfo.Arguments = sb.ToString();
        process.StartInfo.UseShellExecute = true;

        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode == 0 && OpenFolderWhenDownloaded)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }
    }

    partial void OnOpenFolderWhenDownloadedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.OpenFolderWhenDownloaded, value);
}
