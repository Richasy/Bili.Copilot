// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

/// <summary>
/// 直播源列表响应类型.
/// </summary>
internal sealed class LiveFeedResponse
{
    /// <summary>
    /// 直播源卡片列表.
    /// </summary>
    [JsonPropertyName("card_list")]
    public List<LiveFeedCard> CardList { get; set; }
}

/// <summary>
/// 直播源卡片.
/// </summary>
internal sealed class LiveFeedCard
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
internal sealed class LiveFeedCardData
{
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

/// <summary>
/// 直播源推荐中我关注的直播间.
/// </summary>
internal sealed class LiveFeedRoom : LiveRoomBase
{
    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("roomid")]
    public long RoomId { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("uname")]
    public string UserName { get; set; }

    /// <summary>
    /// 用户头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string UserAvatar { get; set; }

    /// <summary>
    /// 直播开始时间.
    /// </summary>
    [JsonPropertyName("live_time")]
    public long LiveStartTime { get; set; }

    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("area_v2_id")]
    public long AreaId { get; set; }

    /// <summary>
    /// 分区名.
    /// </summary>
    [JsonPropertyName("area_v2_name")]
    public string AreaName { get; set; }

    /// <summary>
    /// 父分区名.
    /// </summary>
    [JsonPropertyName("area_v2_parent_name")]
    public string ParentAreaName { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("area_v2_parent_id")]
    public long ParentAreaId { get; set; }

    /// <summary>
    /// 直播标签名.
    /// </summary>
    [JsonPropertyName("live_tag_name")]
    public string LiveTagName { get; set; }

    /// <summary>
    /// 是否为特别关注，0-否，1-是.
    /// </summary>
    [JsonPropertyName("special_attention")]
    public int SpecialAttention { get; set; }

    /// <summary>
    /// 是否官方认证，0-否，1-是.
    /// </summary>
    [JsonPropertyName("official_verify")]
    public int OfficialVerify { get; set; }
}

/// <summary>
/// 直播间卡片.
/// </summary>
internal sealed class LiveRoomCard : LiveRoomBase
{
    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long? RoomId { get; set; }

    /// <summary>
    /// 所属分区Id.
    /// </summary>
    [JsonPropertyName("area_id")]
    public long? AreaId { get; set; }

    /// <summary>
    /// 显示分区名.
    /// </summary>
    [JsonPropertyName("area_name")]
    public string? AreaName { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("parent_area_id")]
    public long? ParentAreaId { get; set; }

    /// <summary>
    /// 父分区名称.
    /// </summary>
    [JsonPropertyName("parent_area_name")]
    public string? ParentAreaName { get; set; }

    /// <summary>
    /// 封面左侧文本，指用户名.
    /// </summary>
    [JsonPropertyName("cover_left_style")]
    public LiveCoverContent? CoverLeftContent { get; set; }

    /// <summary>
    /// 封面右侧文本，指观看人数.
    /// </summary>
    [JsonPropertyName("cover_right_style")]
    public LiveCoverContent? CoverRightContent { get; set; }

    /// <summary>
    /// 反馈列表.
    /// </summary>
    [JsonPropertyName("feedback")]
    public IList<LiveFeedback>? Feedback { get; set; }

    /// <summary>
    /// 序号.
    /// </summary>
    [JsonPropertyName("index")]
    public long? Index { get; set; }

    /// <summary>
    /// 是否隐藏反馈.0-不隐藏，1-隐藏.
    /// </summary>
    [JsonPropertyName("is_hide_feedback")]
    public int? IsHideFeedback { get; set; }

    [JsonPropertyName("uname")]
    public string? UpName { get; set; }

    /// <summary>
    /// 封面内容样式.
    /// </summary>
    internal sealed class LiveCoverContent
    {
        /// <summary>
        /// 封面文本.
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    /// <summary>
    /// 反馈.
    /// </summary>
    internal sealed class LiveFeedback
    {
        /// <summary>
        /// 标题.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 副标题.
        /// </summary>
        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        /// <summary>
        /// 类型.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// 理由集合.
        /// </summary>
        [JsonPropertyName("reasons")]
        public IList<LiveFeedbackReason>? Reasons { get; set; }

        /// <summary>
        /// 直播反馈理由.
        /// </summary>
        internal sealed class LiveFeedbackReason
        {
            /// <summary>
            /// 标识符.
            /// </summary>
            [JsonPropertyName("id")]
            public long? Id { get; set; }

            /// <summary>
            /// 名称.
            /// </summary>
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Id类型.
            /// </summary>
            [JsonPropertyName("id_type")]
            public string? IdType { get; set; }
        }
    }
}

/// <summary>
/// 直播源关注用户列表.
/// </summary>
internal sealed class LiveFeedFollowUserList
{
    /// <summary>
    /// 列表数据.
    /// </summary>
    [JsonPropertyName("list")]
    public IList<LiveFeedRoom>? List { get; set; }
}

/// <summary>
/// 直播间基类.
/// </summary>
internal class LiveRoomBase
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("uid")]
    public long? UserId { get; set; }

    /// <summary>
    /// 直播间封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>
    /// 直播间标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 清晰度描述列表.
    /// </summary>
    [JsonPropertyName("quality_description")]
    public IList<LiveQualityDescription>? QualityDescriptionList { get; set; }

    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }

    /// <summary>
    /// 分组标识符.
    /// </summary>
    [JsonPropertyName("group_id")]
    public int? GroupId { get; set; }

    /// <summary>
    /// 在线观看人数.
    /// </summary>
    [JsonPropertyName("online")]
    public int? ViewerCount { get; set; }

    /// <summary>
    /// 播放地址.
    /// </summary>
    [JsonPropertyName("play_url")]
    public string? PlayUrl { get; set; }

    /// <summary>
    /// H265播放地址.
    /// </summary>
    [JsonPropertyName("play_url_h265")]
    public string? H265PlayUrl { get; set; }

    /// <summary>
    /// 当前清晰度.
    /// </summary>
    [JsonPropertyName("current_quality")]
    public int? CurrentQuality { get; set; }

    /// <summary>
    /// 对决的直播间Id.
    /// </summary>
    [JsonPropertyName("pk_id")]
    public int? PkId { get; set; }

    /// <summary>
    /// 直播间地址.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; set; }
}

/// <summary>
/// 直播清晰度说明.
/// </summary>
internal sealed class LiveQualityDescription
{
    /// <summary>
    /// 清晰度标识.
    /// </summary>
    [JsonPropertyName("qn")]
    public int? Quality { get; set; }

    /// <summary>
    /// 清晰度说明.
    /// </summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveQualityDescription description && Quality == description.Quality;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Quality);
}
