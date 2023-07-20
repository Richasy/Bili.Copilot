// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 打开插件接口.
/// </summary>
public interface IOpenPlugin : IPluginBase
{
    /// <summary>
    /// 是否可以打开.
    /// </summary>
    /// <returns>结果.</returns>
    bool CanOpen();

    /// <summary>
    /// 打开.
    /// </summary>
    /// <returns>打开的结果.</returns>
    OpenResult Open();

    /// <summary>
    /// 打开单项.
    /// </summary>
    /// <returns>打开结果.</returns>
    OpenResult OpenItem();
}
