﻿namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

public class DeviceStreamBase
{
    public string   DeviceFriendlyName  { get; }
    public string   Url                 { get; protected set; }

    public DeviceStreamBase(string deviceFriendlyName) => DeviceFriendlyName = deviceFriendlyName;
}
