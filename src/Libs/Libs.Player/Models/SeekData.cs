// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 表示一个用于定位的数据.
/// </summary>
internal class SeekData
{
    /// <summary>
    /// 初始化 SeekData 类的新实例.
    /// </summary>
    /// <param name="ms">定位的毫秒数.</param>
    /// <param name="forward">定位的方向是否为向前.</param>
    /// <param name="accurate">定位是否需要精确.</param>
    public SeekData(int ms, bool forward, bool accurate)
    {
        Ms = ms;
        Forward = forward && !accurate;
        Accurate = accurate;
    }

    /// <summary>
    /// 获取或设置定位的毫秒数.
    /// </summary>
    public int Ms { get; set; }

    /// <summary>
    /// 获取或设置一个值，指示定位的方向是否为向前.
    /// </summary>
    public bool Forward { get; set; }

    /// <summary>
    /// 获取或设置一个值，指示定位是否需要精确.
    /// </summary>
    public bool Accurate { get; set; }
}
