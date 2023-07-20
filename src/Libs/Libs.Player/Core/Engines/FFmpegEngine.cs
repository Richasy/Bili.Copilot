// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Misc;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.Core.Engines;

/// <summary>
/// FFmpeg 引擎类.
/// </summary>
public class FFmpegEngine
{
#pragma warning disable SA1401 // Fields should be private
    internal static unsafe av_log_set_callback_callback LogFFmpeg = (p0, level, format, vl) =>
#pragma warning restore SA1401 // Fields should be private
    {
        if (level > av_log_get_level())
        {
            return;
        }

        var buffer = stackalloc byte[AV_LOG_BUFFER_SIZE];
        var printPrefix = 1;
        av_log_format_line2(p0, level, format, vl, buffer, AV_LOG_BUFFER_SIZE, &printPrefix);
        var line = Utils.BytePtrToStringUtf8(buffer);

        Logger.Output($"{DateTime.Now.ToString(Engine.Config.LogDateTimeFormat)} | FFmpeg | {(FFmpegLogLevel)level,-7} | {line.Trim()}");
    };

    private const int AV_LOG_BUFFER_SIZE = 5 * 1024;

    internal FFmpegEngine()
    {
        try
        {
            Engine.Log.Info($"从 '{Engine.Config.FFmpegPath}' 加载 FFmpeg 库");
            Folder = Utils.GetFolderPath(Engine.Config.FFmpegPath);
            RootPath = Folder;
            var ver = avformat_version();
            Version = $"{ver >> 16}.{ver >> 8 & 255}.{ver & 255}";

            if (Engine.Config.FFmpegDevices)
            {
                try
                {
                    avdevice_register_all();
                    DevicesLoaded = true;
                }
                catch
                {
                    Engine.Log.Error("FFmpeg 加载 avdevices/avfilters/postproc 失败");
                }
            }

            try
            {
                avfilter_version();
                FiltersLoaded = true;
            }
            catch
            {
                FiltersLoaded = false;
            }

            SetLogLevel();

            Engine.Log.Info($"FFmpeg 已加载 (位置: {Folder}, 版本: {Version}) [设备: {(DevicesLoaded ? "是" : "否")}, 过滤器: {(FiltersLoaded ? "是" : "否")}]");
        }
        catch (Exception e)
        {
            Engine.Log.Error($"加载 FFmpeg 库 '{Engine.Config.FFmpegPath}' 失败\r\n{e.Message}\r\n{e.StackTrace}");
            throw new Exception($"加载 FFmpeg 库 '{Engine.Config.FFmpegPath}' 失败");
        }
    }

    /// <summary>
    /// 获取 FFmpeg 文件夹路径.
    /// </summary>
    public string Folder { get; private set; }

    /// <summary>
    /// 获取 FFmpeg 版本号.
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// 获取或设置一个值，指示过滤器是否已加载.
    /// </summary>
    public bool FiltersLoaded { get; set; }

    /// <summary>
    /// 获取或设置一个值，指示设备是否已加载.
    /// </summary>
    public bool DevicesLoaded { get; set; }

    internal static void SetLogLevel()
    {
        if (Engine.Config.FFmpegLogLevel != FFmpegLogLevel.Quiet)
        {
            av_log_set_level((int)Engine.Config.FFmpegLogLevel);
            av_log_set_callback(LogFFmpeg);
        }
        else
        {
            av_log_set_level((int)FFmpegLogLevel.Quiet);
            av_log_set_callback(null);
        }
    }

    internal static unsafe string ErrorCodeToMsg(int error)
    {
        var buffer = stackalloc byte[AV_LOG_BUFFER_SIZE];
        av_strerror(error, buffer, AV_LOG_BUFFER_SIZE);
        return Utils.BytePtrToStringUtf8(buffer);
    }
}
