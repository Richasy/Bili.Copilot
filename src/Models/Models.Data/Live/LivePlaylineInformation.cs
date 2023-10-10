// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Live;

/// <summary>
/// 直播解码信息.
/// </summary>
public sealed class LivePlaylineInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlaylineInformation"/> class.
    /// </summary>
    /// <param name="name">线路名.</param>
    /// <param name="quality">清晰度.</param>
    /// <param name="acceptQualities">支持的清晰度列表.</param>
    /// <param name="urls">地址列表.</param>
    public LivePlaylineInformation(
        string name,
        int quality,
        List<int> acceptQualities,
        List<LivePlayUrl> urls)
    {
        Name = name;
        Quality = quality;
        AcceptQualities = acceptQualities;
        Urls = urls;
    }

    /// <summary>
    /// 解码名.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 清晰度标识.
    /// </summary>
    public int Quality { get; }

    /// <summary>
    /// 支持的清晰度标识.
    /// </summary>
    public List<int> AcceptQualities { get; }

    /// <summary>
    /// 播放地址列表.
    /// </summary>
    public List<LivePlayUrl> Urls { get; }
}
