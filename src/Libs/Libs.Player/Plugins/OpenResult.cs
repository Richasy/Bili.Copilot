// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Plugins;

/// <summary>
/// 打开结果.
/// </summary>
public class OpenResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenResult"/> class.
    /// </summary>
    public OpenResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenResult"/> class.
    /// </summary>
    /// <param name="error">错误信息.</param>
    public OpenResult(string error)
        => Error = error;

    /// <summary>
    /// 错误信息.
    /// </summary>
    public string Error { get; internal set; }

    /// <summary>
    /// 是否成功.
    /// </summary>
    public bool IsSuccess => Error == null;
}
