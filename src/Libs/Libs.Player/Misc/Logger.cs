// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;

namespace Bili.Copilot.Libs.Player.Misc;

/// <summary>
/// 日志记录器.
/// </summary>
public static class Logger
{
    private static readonly ConcurrentQueue<byte[]> _fileData = new();
    private static readonly object _lockFileStream = new();
    private static readonly Dictionary<LogLevel, string> _logLevels = new();
    private static string _lastOutput = string.Empty;
    private static bool _fileTaskRunning;
    private static FileStream _fileStream;

    static Logger()
    {
        foreach (LogLevel logLevel in Enum.GetValues(typeof(LogLevel)))
        {
            _logLevels.Add(logLevel, logLevel.ToString().PadRight(5, ' '));
        }
    }

    /// <summary>
    /// 是否可以记录错误日志.
    /// </summary>
    public static bool CanError => Engine.Config.LogLevel >= LogLevel.Error;

    /// <summary>
    /// 是否可以记录警告日志.
    /// </summary>
    public static bool CanWarn => Engine.Config.LogLevel >= LogLevel.Warn;

    /// <summary>
    /// 是否可以记录信息日志.
    /// </summary>
    public static bool CanInfo => Engine.Config.LogLevel >= LogLevel.Info;

    /// <summary>
    /// 是否可以记录调试日志.
    /// </summary>
    public static bool CanDebug => Engine.Config.LogLevel >= LogLevel.Debug;

    /// <summary>
    /// 是否可以记录跟踪日志.
    /// </summary>
    public static bool CanTrace => Engine.Config.LogLevel >= LogLevel.Trace;

    internal static Action<string> Output { get; private set; } = DevNullPtr;

    /// <summary>
    /// 强制将缓存的文件数据写入文件.
    /// </summary>
    public static void ForceFlush()
    {
        if (!_fileTaskRunning && _fileStream != null)
        {
            FlushFileData();
        }
    }

    internal static void Log(string msg, LogLevel logLevel)
    {
        if (logLevel <= Engine.Config.LogLevel)
        {
            Output($"{DateTime.Now.ToString(Engine.Config.LogDateTimeFormat)} | {_logLevels[logLevel]} | {msg}");
        }
    }

    internal static void SetOutput()
    {
        var output = Engine.Config.LogOutput;

        if (string.IsNullOrEmpty(output))
        {
            if (_lastOutput != string.Empty)
            {
                Output = DevNullPtr;
                _lastOutput = string.Empty;
            }
        }
        else if (output.StartsWith(":"))
        {
            if (output == ":console")
            {
                if (_lastOutput != ":console")
                {
                    Output = Console.WriteLine;
                    _lastOutput = ":console";
                }
            }
            else if (output == ":debug")
            {
                if (_lastOutput != ":debug")
                {
                    Output = DebugPtr;
                    _lastOutput = ":debug";
                }
            }
            else
            {
                throw new Exception("Invalid log output");
            }
        }
        else
        {
            lock (_lockFileStream)
            {
                // Flush File Data on Previously Opened File Stream
                if (_fileStream != null)
                {
                    while (_fileData.TryDequeue(out var data))
                    {
                        _fileStream.Write(data, 0, data.Length);
                    }

                    _fileStream.Dispose();
                }

                var dir = Path.GetDirectoryName(output);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                _fileStream = new FileStream(output, Engine.Config.LogAppend ? FileMode.Append : FileMode.Create, FileAccess.Write);
                if (_lastOutput != ":file")
                {
                    Output = FilePtr;
                    _lastOutput = ":file";
                }
            }
        }
    }

    private static void DebugPtr(string msg) => System.Diagnostics.Debug.WriteLine(msg);

    private static void DevNullPtr(string msg)
    {
    }

    private static void FilePtr(string msg)
    {
        _fileData.Enqueue(Encoding.UTF8.GetBytes($"{msg}\r\n"));

        if (!_fileTaskRunning && _fileData.Count > Engine.Config.LogCachedLines)
        {
            FlushFileData();
        }
    }

    private static void FlushFileData()
    {
        _fileTaskRunning = true;

        Task.Run(() =>
        {
            lock (_lockFileStream)
            {
                while (_fileData.TryDequeue(out var data))
                {
                    _fileStream.Write(data, 0, data.Length);
                }

                _fileStream.Flush();
            }

            _fileTaskRunning = false;
        });
    }
}
