// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
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
    public VideoConnectorViewModel(PlayerViewModel parent)
    {
        Parent = parent;
        _comments = this.Get<CommentMainViewModel>();
        Downloader = this.Get<DownloadViewModel>();
        AI = this.Get<AIViewModel>();
    }

    public async Task InitializeAsync(MediaSnapshot snapshot, IMpvMediaSourceResolver sourceResolver)
    {
        _snapshot = snapshot;
        ClearView();
        IsLoading = true;
        await LoadItemDetailAsync();
        var prev = FindPrevVideo();
        var next = FindNextVideo();
        var prevName = prev is VideoPart part ? part.Identifier.Title : prev is VideoInformation video ? video.Identifier.Title : default;
        var nextName = next is VideoPart part2 ? part2.Identifier.Title : next is VideoInformation video2 ? video2.Identifier.Title : default;
        var initArgs = new PlaylistInitializedEventArgs(prev != null, next != null, prevName, nextName);
        PlaylistInitialized?.Invoke(this, initArgs);
        IsLoading = false;
    }

    public void InitializeDownloader(List<PlayerFormatInformation> formats)
    {
        Downloader.Clear();
        Downloader.InitializeMetas(
            GetWebLink(),
            formats.AsReadOnly(),
            _view.Parts?.Count > 1 ? _view.Parts.AsReadOnly() : default,
            _view.Parts.IndexOf(_part) + 1);
    }

    public async Task PlayNextAsync()
    {
        if (IsLoading)
        {
            return;
        }

        var next = FindNextVideo();
        if (next is null)
        {
            return;
        }

        if (next is VideoPart part)
        {
            _snapshot.PreferPart = part;
            Parent.InitializeCommand.Execute(_snapshot);
            return;
        }
        else if (next is VideoInformation video)
        {
            Parent.InitializeCommand.Execute(new MediaSnapshot(video, IsPrivatePlay) { Playlist = _snapshot.Playlist, Type = BiliMediaType.Video });
        }

        await Task.CompletedTask;
    }

    public async Task PlayPreviousAsync()
    {
        if (IsLoading)
        {
            return;
        }

        var next = FindPrevVideo();
        if (next is null)
        {
            return;
        }

        if (next is VideoPart part)
        {
            _snapshot.PreferPart = part;
            Parent.InitializeCommand.Execute(_snapshot);
            return;
        }
        else if (next is VideoInformation video)
        {
            Parent.InitializeCommand.Execute(new MediaSnapshot(video, IsPrivatePlay) { Playlist = _snapshot.Playlist, Type = BiliMediaType.Video });
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    private void OpenPanel()
        => RequestOpenExtraPanel?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void SelectSection(IPlayerSectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        SelectedSection.TryFirstLoadCommand.Execute(default);
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
            _part = FindInitialPart(_snapshot.PreferPart?.Identifier.Id);
            UpdateProperties();
            InitializeView(_view);
            _comments?.Initialize(_view.Information.Identifier.Id, Richasy.BiliKernel.Models.CommentTargetType.Video, Richasy.BiliKernel.Models.CommentSortType.Hot);
            InitializeSections();
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
            return _view.Parts[0];
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
        Subtitle = string.IsNullOrEmpty(_view.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description)) ? _view.Information.Publisher.User.Name : _view.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description);
        Description = _view.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description) ?? string.Empty;
        Cover = _view.Information.Identifier.Cover?.Uri;
        var args = new PlayerInformationUpdatedEventArgs(Title, Subtitle, Cover);
        PropertiesUpdated?.Invoke(this, args);
    }
}
