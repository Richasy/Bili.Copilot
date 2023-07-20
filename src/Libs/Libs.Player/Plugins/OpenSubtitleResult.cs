﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 打开字幕的结果.
/// </summary>
public class OpenSubtitleResult : OpenResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSubtitleResult"/> class.
    /// </summary>
    /// <param name="extStream">外部字幕流.</param>
    /// <param name="error">错误信息.</param>
    public OpenSubtitleResult(ExternalSubtitleStream extStream, string error = default)
        : base(error)
        => ExternalSubtitlesStream = extStream;

    public ExternalSubtitleStream ExternalSubtitleStream;
}
