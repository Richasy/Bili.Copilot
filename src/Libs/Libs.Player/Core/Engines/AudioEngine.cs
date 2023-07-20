// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Misc;
using SharpGen.Runtime;
using SharpGen.Runtime.Win32;

using Vortice.MediaFoundation;
using static Vortice.XAudio2.XAudio2;

namespace Bili.Copilot.Libs.Player.Core.Engines;

/// <summary>
/// 音频引擎.
/// </summary>
public sealed class AudioEngine : CallbackBase, IMMNotificationClient
{
    private readonly object _lockDevices = new();
    private readonly object _locker = new();
    private IMMDeviceEnumerator _deviceEnum;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioEngine"/> class.
    /// </summary>
    public AudioEngine()
    {
        if (Engine.Config.DisableAudio)
        {
            Failed = true;
            return;
        }

        EnumerateDevices();
    }

    /// <summary>
    /// 默认音频设备名称.
    /// </summary>
    public string DefaultDeviceName { get; private set; } = "Default";

    /// <summary>
    /// 默认音频设备 ID.
    /// </summary>
    public string DefaultDeviceId { get; private set; } = "0";

    /// <summary>
    /// 是否未找到音频设备或音频初始化失败.
    /// </summary>
    public bool Failed { get; private set; }

    /// <summary>
    /// 当前音频设备名称.
    /// </summary>
    public string CurrentDeviceName { get; private set; } = "Default";

    /// <summary>
    /// 当前音频设备 ID.
    /// </summary>
    public string CurrentDeviceId { get; private set; } = "0";

    /// <summary>
    /// 音频捕获设备列表.
    /// </summary>
    public ObservableCollection<AudioDevice> CapDevices { get; set; } = new();

    /// <summary>
    /// 音频设备列表.
    /// </summary>
    public ObservableCollection<string> Devices { get; private set; } = new();

    /// <summary>
    /// 刷新音频捕获设备列表.
    /// </summary>
    public void RefreshCapDevices() => AudioDevice.RefreshDevices();

    /// <summary>
    /// 根据设备名称获取设备 ID.
    /// </summary>
    /// <param name="deviceName">设备名称.</param>
    /// <returns>设备 ID.</returns>
    public string GetDeviceId(string deviceName)
    {
        if (deviceName == DefaultDeviceName)
        {
            return DefaultDeviceId;
        }

        foreach (var device in _deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceStates.Active))
        {
            if (device.FriendlyName.ToLower() != deviceName.ToLower())
            {
                continue;
            }

            return device.Id;
        }

        throw new Exception("指定的音频设备不存在.");
    }

    /// <summary>
    /// 根据设备 ID 获取设备名称.
    /// </summary>
    /// <param name="deviceId">设备 ID.</param>
    /// <returns>设备名称.</returns>
    public string GetDeviceName(string deviceId)
    {
        if (deviceId == DefaultDeviceId)
        {
            return DefaultDeviceName;
        }

        foreach (var device in _deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceStates.Active))
        {
            if (device.Id.ToLower() != deviceId.ToLower())
            {
                continue;
            }

            return device.FriendlyName;
        }

        throw new Exception("指定的音频设备不存在.");
    }

    /// <inheritdoc/>
    public void OnDeviceStateChanged(string pwstrDeviceId, int newState)
        => RefreshDevices();

    /// <inheritdoc/>
    public void OnDeviceAdded(string pwstrDeviceId)
        => RefreshDevices();

    /// <inheritdoc/>
    public void OnDeviceRemoved(string pwstrDeviceId)
        => RefreshDevices();

    /// <inheritdoc/>
    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string pwstrDefaultDeviceId)
        => RefreshDevices();

    /// <inheritdoc/>
    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
    {
    }

    private void EnumerateDevices()
    {
        try
        {
            _deviceEnum = new IMMDeviceEnumerator();

            var defaultDevice = _deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (defaultDevice == null)
            {
                Failed = true;
                return;
            }

            lock (_lockDevices)
            {
                Devices.Clear();
                Devices.Add(DefaultDeviceName);
                foreach (var device in _deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceStates.Active))
                {
                    Devices.Add(device.FriendlyName);
                }
            }

            CurrentDeviceId = defaultDevice.Id;
            CurrentDeviceName = defaultDevice.FriendlyName;

            if (Logger.CanInfo)
            {
                var dump = string.Empty;
                foreach (var device in _deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceStates.Active))
                {
                    dump += $"{device.Id} | {device.FriendlyName} {(defaultDevice.Id == device.Id ? "*" : string.Empty)}\r\n";
                }

                Engine.Log.Info($"音频设备\r\n{dump}");
            }

            var xaudio2 = XAudio2Create();

            if (xaudio2 == null)
            {
                Failed = true;
            }
            else
            {
                xaudio2.Dispose();
            }

            _deviceEnum.RegisterEndpointNotificationCallback(this);
        }
        catch
        {
            Failed = true;
        }
    }

    private void RefreshDevices()
    {
        // 刷新设备并初始化音频播放器（如果需要）
        lock (_locker)
        {
            Utils.UI(() =>
            {
                lock (_lockDevices)
                {
                    Devices.Clear();
                    Devices.Add(DefaultDeviceName);
                    foreach (var device in _deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceStates.Active))
                    {
                        Devices.Add(device.FriendlyName);
                    }
                }

                foreach (var player in Engine.Players)
                {
                    if (!Devices.Contains(player.Audio.Device))
                    {
                        player.Audio.Device = DefaultDeviceName;
                    }
                    else
                    {
                        player.Audio.RaiseDevice();
                    }
                }
            });

            var defaultDevice = _deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (defaultDevice != null)
            {
                CurrentDeviceId = defaultDevice.Id;
                CurrentDeviceName = defaultDevice.FriendlyName;
            }
        }
    }
}
