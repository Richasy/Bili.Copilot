// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Player;

namespace Bili.Copilot.Models.Data.Live;

/// <summary>
/// 直播媒体信息.
/// </summary>
public sealed class LiveMediaInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveMediaInformation"/> class.
    /// </summary>
    /// <param name="id">直播间 Id.</param>
    /// <param name="formats">直播清晰度格式.</param>
    /// <param name="lines">直播线路.</param>
    public LiveMediaInformation(
        string id,
        List<FormatInformation> formats,
        List<LivePlaylineInformation> lines)
    {
        Id = id;
        Formats = formats;
        Lines = lines;
    }

    /// <summary>
    /// 直播间 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 格式列表.
    /// </summary>
    public List<FormatInformation> Formats { get; }

    /// <summary>
    /// 播放线路列表.
    /// </summary>
    public List<LivePlaylineInformation> Lines { get; }
}
