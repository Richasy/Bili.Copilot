using System;
using Vortice.MediaFoundation;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

public class AudioDevice : DeviceBase<AudioDeviceStream>
{
    public AudioDevice(string friendlyName, string symbolicLink) : base(friendlyName, symbolicLink)
        => Url = $"fmt://dshow?audio={FriendlyName}";

    public static void RefreshDevices()
    {
        Utils.UI(() =>
        {
            Engine.Audio.CapDevices.Clear();

            var devices = MediaFactory.MFEnumAudioDeviceSources();
                foreach (var device in devices)
                    try { Engine.Audio.CapDevices.Add(new AudioDevice(device.FriendlyName, device.SymbolicLink)); } catch(Exception) { }
        });
    }
}
