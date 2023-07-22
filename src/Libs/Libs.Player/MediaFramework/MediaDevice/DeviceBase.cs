// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Core;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

/// <summary>
/// 设备基类.
/// </summary>
public class DeviceBase : DeviceBase<DeviceStreamBase>
{
    /// <summary>
    /// 构造函数，用于初始化 DeviceBase 类的新实例.
    /// </summary>
    /// <param name="friendlyName">设备友好名称.</param>
    /// <param name="symbolicLink">设备符号链接.</param>
    public DeviceBase(string friendlyName, string symbolicLink)
        : base(friendlyName, symbolicLink)
    {
    }
}

/// <summary>
/// 设备基类，泛型参数为 T，T 必须是 DeviceStreamBase 的派生类.
/// </summary>
/// <typeparam name="T">设备流的类型.</typeparam>
public class DeviceBase<T>
    where T : DeviceStreamBase
{
    /// <summary>
    /// 构造函数，用于初始化 DeviceBase&lt;T&gt; 类的新实例.
    /// </summary>
    /// <param name="friendlyName">设备友好名称.</param>
    /// <param name="symbolicLink">设备符号链接.</param>
    public DeviceBase(string friendlyName, string symbolicLink)
    {
        FriendlyName = friendlyName;
        SymbolicLink = symbolicLink;
        Engine.Log.Debug($"[{(this is AudioDevice ? "Audio" : "Video")}Device] {friendlyName}");
    }

    /// <summary>
    /// 获取设备的友好名称.
    /// </summary>
    public string FriendlyName { get; }

    /// <summary>
    /// 获取设备的符号链接.
    /// </summary>
    public string SymbolicLink { get; }

    /// <summary>
    /// 获取或设置设备的流列表.
    /// </summary>
    public IList<T> Streams { get; protected set; }

    /// <summary>
    /// 获取或设置设备的 URL，默认为默认 URL.
    /// </summary>
    public string Url { get; protected set; }

    /// <summary>
    /// 返回设备的友好名称.
    /// </summary>
    /// <returns>设备的友好名称.</returns>
    public override string ToString() => FriendlyName;
}
