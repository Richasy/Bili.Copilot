// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel.Player;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoConnectorViewModel : ViewModelBase, IPlayerConnectorViewModel
{
    public async Task InitializeAsync(MediaSnapshot snapshot, IMpvMediaSourceResolver sourceResolver)
    {
        Playlist.Clear();
        _snapshot = snapshot;
        if (snapshot.Playlist?.Count > 0)
        {
            foreach (var item in snapshot.Playlist)
            {
                var vm = new PlaylistMediaViewModel(snapshot, PlayItem);
                vm.IsPlaying = item.Video.Identifier.Id == vm.Data.Video.Identifier.Id;
                Playlist.Add(vm);
            }

            var index = GetCurrentIndex();
            var prev = index <= 0 ? null : Playlist[index - 1];
            var next = index >= Playlist.Count - 1 ? null : Playlist[index + 1];
            var initArgs = new PlaylistInitializedEventArgs(prev != null, next != null, prev?.Name, next?.Name);
            PlaylistInitialized?.Invoke(this, initArgs);
        }

        IsPlaylistAvailable = Playlist.Count > 1;
        await LoadItemDetailAsync();
    }

    public Task PlayNextAsync()
    {
        var index = GetCurrentIndex();
        if (index < Playlist.Count - 1)
        {
            var next = Playlist[index + 1];
            PlayItem(next);
        }

        return Task.CompletedTask;
    }

    public Task PlayPreviousAsync()
    {
        var index = GetCurrentIndex();
        if (index > 0)
        {
            var next = Playlist[index - 1];
            PlayItem(next);
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private void OpenPanel()
        => RequestOpenExtraPanel?.Invoke(this, EventArgs.Empty);

    internal int GetCurrentIndex()
    {
        var item = Playlist.FirstOrDefault(i => i.IsPlaying);
        if (item is not null)
        {
            return Playlist.IndexOf(item);
        }

        return 0;
    }

    private void PlayItem(PlaylistMediaViewModel vm)
    {
        if (vm.Data.Video.Identifier.Id == _snapshot.Video.Identifier.Id)
        {
            return;
        }

        NewMediaRequest?.Invoke(this, vm.Data);
    }

    private async Task LoadItemDetailAsync()
    {
        var service = this.Get<IPlayerService>();
        try
        {
            _view = await service.GetVideoPageDetailAsync(_snapshot.Video.Identifier);
            _part = FindInitialPart(default);
            UpdateProperties();
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "获取视频信息失败");
        }
    }

    private VideoPart? FindInitialPart(string? initialPartId = default)
    {
        if (_view.Parts?.Count == 1)
        {
            return _view.Parts.First();
        }

        VideoPart? part = default;
        if (!string.IsNullOrEmpty(initialPartId))
        {
            part = _view.Parts.FirstOrDefault(p => p.Identifier.Id == initialPartId);
        }

        if (part == null)
        {
            var historyPartId = _view.Progress?.Cid;
            var autoLoadHistory = SettingsToolkit.ReadLocalSetting(SettingNames.AutoLoadHistory, true);
            if (!string.IsNullOrEmpty(historyPartId) && autoLoadHistory)
            {
                part = _view.Parts.FirstOrDefault(p => p.Identifier.Id == historyPartId);
            }
        }

        return part ?? _view.Parts.FirstOrDefault();
    }

    private void UpdateProperties()
    {
        Title = _view.Information.Identifier.Title;
        Subtitle = _view.Information.Publisher.User.Name;
        Cover = _view.Information.Identifier.Cover?.Uri;
        var args = new PlayerInformationUpdatedEventArgs(Title, Subtitle, Cover);
        PropertiesUpdated?.Invoke(this, args);
    }
}
