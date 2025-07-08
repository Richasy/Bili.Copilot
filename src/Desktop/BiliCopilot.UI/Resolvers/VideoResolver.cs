// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel.Core.Models;
using Richasy.MpvKernel.Player.Models;

namespace BiliCopilot.UI.Resolvers;

public sealed class VideoResolver(MediaSnapshot snapshot) : MediaResolverBase
{
    private readonly IPlayerService _playerService = GlobalDependencies.Kernel.GetRequiredService<IPlayerService>();
    private VideoPlayerView? _view;
    private DashMediaInformation? _mediaInfo;
    private string _cid;
    private TaskCompletionSource<bool>? _initTask;
    private CancellationTokenSource? _initCts;
    private TaskCompletionSource<bool>? _playTask;
    private CancellationTokenSource? _playCts;

    public override async Task<MpvMediaSource> GetSourceAsync()
    {
        var volume = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerVolume, 100d);
        var speed = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerSpeed, 1d);
        var options = new MpvPlayOptions
        {
            WindowHandle = WindowHandle,
            InitialVolume = volume,
            InitialSpeed = speed,
        };
    }

    public async Task SwitchPartAsync(string cid)
    {
        if (_cid == cid)
        {
            return;
        }

        _cid = cid;
        _mediaInfo = default;
        await LoadPlayInfoAsync();
    }

    private async Task LoadVideoInfoAsync()
    {
        if (_view is not null)
        {
            return;
        }

        if (_initTask?.Task.IsCompleted == true)
        {
            return;
        }

        if (_initTask is not null)
        {
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
            await _initTask.Task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
            return;
        }

        _initTask = new TaskCompletionSource<bool>();
        _initCts = new CancellationTokenSource();
        try
        {
            _view = await _playerService.GetVideoPageDetailAsync(snapshot.Media, _initCts.Token);
            _mediaInfo = default;
        }
        catch (Exception ex)
        {
            _initTask.SetException(ex);
            return;
        }

        _initTask.TrySetResult(true);
    }

    private async Task LoadPlayInfoAsync()
    {
        if (_view is null)
        {
            return;
        }

        if (_playTask?.Task.IsCompleted == true)
        {
            return;
        }

        if (_playTask is not null)
        {
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
            await _playTask.Task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
            return;
        }

        try
        {
            _playTask = new TaskCompletionSource<bool>();
            _playCts = new CancellationTokenSource();
            _mediaInfo = await _playerService.GetVideoPlayDetailAsync(snapshot.Media, long.Parse(_cid), _playCts.Token);
        }
        catch (Exception ex)
        {
            _playTask.SetException(ex);
            return;
        }

        _playTask.TrySetResult(true);
    }
}
