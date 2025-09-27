// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using Richasy.MpvKernel.Player;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器连接器视图模型接口。
/// </summary>
public interface IPlayerConnectorViewModel
{
    public Task InitializeAsync(MediaSnapshot snapshot, IMpvMediaSourceResolver sourceResolver);

    public Task PlayNextAsync();

    public Task PlayPreviousAsync();

    public event EventHandler<PlaylistInitializedEventArgs> PlaylistInitialized;

    public event EventHandler<MediaSnapshot> NewMediaRequest;

    public event EventHandler RequestOpenExtraPanel;

    public event EventHandler<PlayerInformationUpdatedEventArgs> PropertiesUpdated;
}
