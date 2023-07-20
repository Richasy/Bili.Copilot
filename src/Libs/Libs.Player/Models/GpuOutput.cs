// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 显卡输出.
/// </summary>
public sealed class GpuOutput
{
    /// <summary>
    /// 显卡 ID 生成器.
    /// </summary>
    public static int GpuOutputIdGenerator { get; set; }

    /// <summary>
    /// 显卡 ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 显卡名称.
    /// </summary>
    public string DeviceName { get; internal set; }

    /// <summary>
    /// 左侧位置.
    /// </summary>
    public int Left { get; internal set; }

    /// <summary>
    /// 顶部位置.
    /// </summary>
    public int Top { get; internal set; }

    /// <summary>
    /// 右侧位置.
    /// </summary>
    public int Right { get; internal set; }

    /// <summary>
    /// 底部位置.
    /// </summary>
    public int Bottom { get; internal set; }

    /// <summary>
    /// 显示区域宽度.
    /// </summary>
    public int Width => Right - Left;

    /// <summary>
    /// 显示区域高度.
    /// </summary>
    public int Height => Bottom - Top;

    /// <summary>
    /// 是否已附加.
    /// </summary>
    public bool IsAttached { get; internal set; }

    /// <summary>
    /// 旋转角度.
    /// </summary>
    public int Rotation { get; internal set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var gcd = Utils.GCD(Width, Height);
        return $"{DeviceName,-20} [Id: {Id,-4}\t, Top: {Top,-4}, Left: {Left,-4}, Width: {Width,-4}, Height: {Height,-4}, Ratio: [" + (gcd > 0 ? $"{Width / gcd}:{Height / gcd}]" : "]");
    }
}
