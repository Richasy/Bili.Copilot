// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 显卡适配器.
/// </summary>
public sealed class GpuAdapter
{
    /// <summary>
    /// GPU适配器支持的最大高度.
    /// </summary>
    public int MaxHeight { get; internal set; }

    /// <summary>
    /// 系统内存大小.
    /// </summary>
    public long SystemMemory { get; internal set; }

    /// <summary>
    /// 显存大小.
    /// </summary>
    public long VideoMemory { get; internal set; }

    /// <summary>
    /// 共享内存大小.
    /// </summary>
    public long SharedMemory { get; internal set; }

    /// <summary>
    /// GPU适配器的ID.
    /// </summary>
    public int Id { get; internal set; }

    /// <summary>
    /// GPU适配器的供应商.
    /// </summary>
    public string Vendor { get; internal set; }

    /// <summary>
    /// GPU适配器的描述.
    /// </summary>
    public string Description { get; internal set; }

    /// <summary>
    /// GPU适配器的本地唯一标识符.
    /// </summary>
    public long Luid { get; internal set; }

    /// <summary>
    /// GPU适配器是否有输出.
    /// </summary>
    public bool HasOutput { get; internal set; }

    /// <summary>
    /// GPU适配器的输出列表.
    /// </summary>
    public List<GpuOutput> Outputs { get; internal set; }

    /// <inheritdoc/>
    public override string ToString()
        => (Vendor + " " + Description).PadRight(40) + $"[ID: {Id,-6}, LUID: {Luid,-6}, DVM: {Utils.GetBytesReadable(VideoMemory),-8}, DSM: {Utils.GetBytesReadable(SystemMemory),-8}, SSM: {Utils.GetBytesReadable(SharedMemory)}]";
}
