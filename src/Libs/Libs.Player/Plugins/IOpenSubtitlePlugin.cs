// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 打开字幕插件的接口.
/// </summary>
public interface IOpenSubtitlePlugin : IPluginBase
{
    OpenSubtitlesResults Open(string url);
    OpenSubtitlesResults Open(Stream iostream);
}
