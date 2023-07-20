// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bili.Copilot.Libs.Player.Plugins;

namespace Bili.Copilot.Libs.Player.Core.Engines;

/// <summary>
/// 插件引擎类，用于加载和管理插件.
/// </summary>
public class PluginsEngine
{
    private Type _pluginBaseType = typeof(PluginBase);

    /// <summary>
    /// 初始化 <see cref="PluginsEngine"/> 类的新实例.
    /// </summary>
    internal PluginsEngine()
    {
        Folder = string.IsNullOrEmpty(Engine.Config.PluginsPath) ? null : Utils.GetFolderPath(Engine.Config.PluginsPath);
        LoadAssemblies();
    }

    /// <summary>
    /// 获取插件类型的字典，键为插件名称，值为插件类型.
    /// </summary>
    public Dictionary<string, PluginType> Types { get; private set; } = new();

    /// <summary>
    /// 获取插件所在的文件夹路径.
    /// </summary>
    public string Folder { get; private set; }

    /// <summary>
    /// 手动加载插件.
    /// </summary>
    /// <param name="assembly">要搜索插件的程序集.</param>
    public void LoadPlugin(Assembly assembly)
    {
        try
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (_pluginBaseType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    // 强制执行静态构造函数（用于提前加载，对于 C# 8.0 和接口的静态属性（例如 DefaultOptions）很有用）
                    // System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                    if (!Types.ContainsKey(type.Name))
                    {
                        Types.Add(type.Name, new PluginType() { Name = type.Name, Type = type, Version = assembly.GetName().Version });
                        Engine.Log.Info($"插件已加载 ({type.Name} - {assembly.GetName().Version})");
                    }
                    else
                    {
                        Engine.Log.Info($"插件已存在 ({type.Name} - {assembly.GetName().Version})");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Engine.Log.Error($"[PluginHandler] [错误] 加载程序集失败 ({e.Message} {Utils.GetRecInnerException(e)})");
        }
    }

    /// <summary>
    /// 加载程序集.
    /// </summary>
    internal void LoadAssemblies()
    {
        // 加载 FlyleafLib 的嵌入插件
        LoadPlugin(Assembly.GetExecutingAssembly());

        // 加载外部插件文件夹
        if (Folder != null && Directory.Exists(Folder))
        {
            var dirs = Directory.GetDirectories(Folder);
            foreach (var dir in dirs)
            {
                foreach (var file in Directory.GetFiles(dir, "*.dll"))
                {
                    LoadPlugin(Assembly.LoadFrom(Path.GetFullPath(file)));
                }
            }
        }
        else
        {
            Engine.Log.Info($"[PluginHandler] 未找到外部插件");
        }
    }
}
