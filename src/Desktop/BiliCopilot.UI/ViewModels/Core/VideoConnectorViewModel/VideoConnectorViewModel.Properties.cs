// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Media;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoConnectorViewModel
{
    public event EventHandler<PlaylistInitializedEventArgs> PlaylistInitialized;
    public event EventHandler<MediaSnapshot> NewMediaRequest;
    public event EventHandler RequestOpenExtraPanel;
    public event EventHandler<PlayerInformationUpdatedEventArgs> PropertiesUpdated;

    private MediaSnapshot _snapshot;
    internal VideoPlayerView _view;
    internal VideoPart? _part;

    [ObservableProperty]
    public partial Uri? Cover { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial string? Subtitle { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsPlaylistAvailable { get; set; }

    [ObservableProperty]
    public partial string? ContainerId { get; set; }

    public ObservableCollection<PlaylistMediaViewModel> Playlist { get; } = [];
}
