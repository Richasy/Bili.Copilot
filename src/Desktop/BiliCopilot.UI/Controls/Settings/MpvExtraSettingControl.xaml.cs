// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.WinUIKernel.Share.Toolkits;

namespace BiliCopilot.UI.Controls.Settings;

public sealed partial class MpvExtraSettingControl : SettingsPageControlBase
{
    private bool _isLoaded;

    public MpvExtraSettingControl()
    {
        InitializeComponent();
    }

    protected override void OnControlLoaded()
    {
        AudioChannelLayoutComboBox.SelectedIndex = (int)ViewModel.PreferAudioChannelLayout;
        ProfileComboBox.SelectedIndex = (int)ViewModel.MpvBuiltInProfile;
        HrSeekComboBox.SelectedIndex = (int)ViewModel.HrSeek;
        CacheSizeBox.Value = ViewModel.MaxCacheSize;
        CacheSecondsBox.Value = ViewModel.MaxCacheSeconds;
        MaxVolumeBox.Value = ViewModel.MaxVolume;

        _isLoaded = true;
    }

    private void OnAudioChannelLayoutChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        var selectedIndex = AudioChannelLayoutComboBox.SelectedIndex;
        ViewModel.PreferAudioChannelLayout = (AudioChannelLayoutType)selectedIndex;
    }

    private void OnCacheSizeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        ViewModel.MaxCacheSize = (int)args.NewValue;
    }

    private void OnCacheSecondsChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        ViewModel.MaxCacheSeconds = (int)args.NewValue;
    }

    private void OnProfileChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        var selectedIndex = ProfileComboBox.SelectedIndex;
        ViewModel.MpvBuiltInProfile = (MpvBuiltInProfile)selectedIndex;
    }

    private void OnHrSeekChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        var selectedIndex = HrSeekComboBox.SelectedIndex;
        ViewModel.HrSeek = (HrSeekType)selectedIndex;
    }

    private void OnMaxVolumeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        ViewModel.MaxVolume = (int)args.NewValue;
    }

    private async void OnCacheDirButtonClick(object sender, RoutedEventArgs e)
    {
        var folder = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
        if (folder != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.CacheDir, folder.Path);
            ViewModel.CacheDirDescription = folder.Path;
            ViewModel.HasCustomCacheDir = true;
        }
    }

    private void OnRemoveCacheDirButtonClick(object sender, RoutedEventArgs e)
    {
        SettingsToolkit.DeleteLocalSetting(SettingNames.CacheDir);
        ViewModel.CacheDirDescription = "--demuxer-cache-dir";
        ViewModel.HasCustomCacheDir = false;
    }
}
