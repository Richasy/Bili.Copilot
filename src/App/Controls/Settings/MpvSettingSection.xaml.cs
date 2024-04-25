// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.ViewModels;
using Windows.ApplicationModel;

namespace Bili.Copilot.App.Controls.Settings;

/// <summary>
/// 网页播放器设置部分.
/// </summary>
public sealed partial class MpvSettingSection : SettingSection
{
    private readonly AppViewModel _appViewModel = AppViewModel.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvSettingSection"/> class.
    /// </summary>
    public MpvSettingSection() => InitializeComponent();

    private async void OnDownloadButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var hasPwsh = await AppToolkit.CheckPwshAvailabilityAsync();
        if (!hasPwsh)
        {
            _appViewModel.ShowMessage(ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.NeedDownloadPwsh));
            return;
        }

        _appViewModel.ShowMessage(ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.DownloadMpvTip));
        await Task.Delay(2000);
        var folder = await FileToolkit.PickFolderAsync(_appViewModel.ActivatedWindow);
        if (folder == null)
        {
            return;
        }

        var path = folder.Path;
        try
        {
            await Task.Run(() =>
            {
                var scriptPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", "install-mpv.ps1");
                var process = new Process();
                process.StartInfo.FileName = "pwsh.exe";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy unrestricted -File \"{scriptPath}\" \"{path}\"";
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();
            });

            Environment.SetEnvironmentVariable("Path", $"{path};{Environment.GetEnvironmentVariable("Path")}");
            _appViewModel.RestartCommand.Execute(default);
        }
        catch (Exception)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _appViewModel.ShowMessage(ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.DownloadMpvFailed));
            });

            return;
        }
    }
}
