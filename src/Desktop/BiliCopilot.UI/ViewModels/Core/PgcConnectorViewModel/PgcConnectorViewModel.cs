// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel.Player;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class PgcConnectorViewModel : ViewModelBase, IPlayerConnectorViewModel
{
    public PgcConnectorViewModel(PlayerViewModel parent)
    {
        Parent = parent;
        _comments = this.Get<CommentMainViewModel>();
        Downloader = this.Get<DownloadViewModel>();
    }

    public async Task InitializeAsync(MediaSnapshot snapshot, IMpvMediaSourceResolver sourceResolver)
    {
        _snapshot = snapshot;
        ClearView();
        IsLoading = true;
        await LoadItemDetailAsync();
        _snapshot.Season = _view.Information;
        _snapshot.Episode = _episode;
        var prev = FindPreviousEpisode();
        var next = FindNextEpisode();
        var prevName = prev?.Identifier.Title;
        var nextName = next?.Identifier.Title;
        var initArgs = new PlaylistInitializedEventArgs(prev != null, next != null, prevName, nextName);
        PlaylistInitialized?.Invoke(this, initArgs);
        IsLoading = false;
    }

    public void InitializeDownloader(List<PlayerFormatInformation> formats)
    {
        Downloader.Clear();
        Downloader.InitializeMetas(
            GetEpisodeUrl(),
            GetSeasonUrl(),
            formats.AsReadOnly(),
            _view.Episodes?.Count > 1 ? _view.Episodes.AsReadOnly() : default,
            _view.Episodes.IndexOf(_episode));
    }

    public async Task PlayNextAsync()
    {
        var next = FindNextEpisode();
        if (next is null)
        {
            return;
        }

        _snapshot.Episode = next;
        Parent.InitializeCommand.Execute(_snapshot);
        await Task.CompletedTask;
    }

    public async Task PlayPreviousAsync()
    {
        var prev = FindPreviousEpisode();
        if (prev is null)
        {
            return;
        }

        _snapshot.Episode = prev;
        Parent.InitializeCommand.Execute(_snapshot);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void OpenPanel()
        => RequestOpenExtraPanel?.Invoke(this, EventArgs.Empty);

    private async Task LoadItemDetailAsync()
    {
        var service = this.Get<IPlayerService>();
        try
        {
            _view = await service.GetPgcPageDetailAsync(_snapshot.Season?.Identifier.Id, _snapshot.Episode?.Identifier.Id);
            _episode = FindInitialEpisode(_snapshot.Episode?.Identifier.Id);
            EpisodeTitle = _episode.Identifier.Title;
            EpisodeId = _episode.Identifier.Id;
            UpdateProperties();
            InitializeView(_view);
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            var cid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
            _comments.Initialize(aid, Richasy.BiliKernel.Models.CommentTargetType.Video, Richasy.BiliKernel.Models.CommentSortType.Hot);
            InitializeSections();
        }
        catch (Exception ex)
        {
            this.Get<ILogger<PgcConnectorViewModel>>().LogError(ex, "获取剧集信息失败");
        }
    }

    private void UpdateProperties()
    {
        Title = _episode.Identifier.Title;
        Subtitle = string.IsNullOrEmpty(_episode.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.Subtitle)) ? _view.Information.Identifier.Title : _episode.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.Subtitle);
        Description = _view.Information.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Description) ?? string.Empty;
        Cover = _view.Information.Identifier.Cover?.Uri;
        var args = new PlayerInformationUpdatedEventArgs(Title, Subtitle, Cover);
        PropertiesUpdated?.Invoke(this, args);
    }
}
