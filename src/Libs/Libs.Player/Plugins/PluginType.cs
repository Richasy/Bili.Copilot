// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 插件类型.
/// </summary>
public class PluginType
{
    /// <summary>
    /// 类型.
    /// </summary>
    public Type Type { get; internal set; }

    /// <summary>
    /// 插件名称.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// 插件版本.
    /// </summary>
    public Version Version { get; internal set; }
}
