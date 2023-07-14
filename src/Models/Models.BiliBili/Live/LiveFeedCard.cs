// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播源卡片.
/// </summary>
public class LiveFeedCard
{
    /// <summary>
    /// 直播卡片类型.
    /// </summary>
    [JsonPropertyName("card_type")]
    public string CardType { get; set; }

    /// <summary>
    /// 卡片数据.
    /// </summary>
    [JsonPropertyName("card_data")]
    public LiveFeedCardData CardData { get; set; }
}

/// <summary>
/// 直播源卡片数据.
/// </summary>
public class LiveFeedCardData
{
    /// <summary>
    /// 横幅列表.
    /// </summary>
    [JsonPropertyName("banner_v1")]
    public LiveFeedBannerList Banners { get; set; }

    /// <summary>
    /// 热门分区.
    /// </summary>
    [JsonPropertyName("area_entrance_v1")]
    public LiveFeedHotAreaList HotAreas { get; set; }

    /// <summary>
    /// 我关注的用户列表.
    /// </summary>
    [JsonPropertyName("my_idol_v1")]
    public LiveFeedFollowUserList FollowList { get; set; }

    /// <summary>
    /// 直播卡片.
    /// </summary>
    [JsonPropertyName("small_card_v1")]
    public LiveRoomCard LiveCard { get; set; }
}

