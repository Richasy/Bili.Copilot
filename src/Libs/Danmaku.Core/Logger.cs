// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using Windows.Foundation.Diagnostics;

namespace Danmaku.Core;

internal static class Logger
{
    private static ILogger<DanmakuFrostMaster> _logger;

    public static void SetLogger(ILogger<DanmakuFrostMaster> logger)
    {
        _logger = logger;
    }

    public static void Log(string message, LoggingLevel level = LoggingLevel.Information, [CallerMemberName] string memberName = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
    {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            filePath = Path.GetFileName(filePath);
        }

        message = $"{filePath ?? string.Empty}({lineNumber})->{memberName ?? string.Empty}(): {message ?? string.Empty}";

        if (level == LoggingLevel.Error)
        {
            _logger?.LogError(message);
        }
        else
        {
            _logger?.LogInformation(message);
        }
    }
}
