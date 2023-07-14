// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播间卡片.
/// </summary>
public class LiveRoomCard : LiveRoomBase
{
    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long RoomId { get; set; }

    /// <summary>
    /// 所属分区Id.
    /// </summary>
    [JsonPropertyName("area_id")]
    public long AreaId { get; set; }

    /// <summary>
    /// 显示分区名.
    /// </summary>
    [JsonPropertyName("area_name")]
    public string AreaName { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("parent_area_id")]
    public long ParentAreaId { get; set; }

    /// <summary>
    /// 父分区名称.
    /// </summary>
    [JsonPropertyName("parent_area_name")]
    public string ParentAreaName { get; set; }

    /// <summary>
    /// 封面左侧文本，指用户名.
    /// </summary>
    [JsonPropertyName("cover_left_style")]
    public LiveCoverContent CoverLeftContent { get; set; }

    /// <summary>
    /// 封面右侧文本，指观看人数.
    /// </summary>
    [JsonPropertyName("cover_right_style")]
    public LiveCoverContent CoverRightContent { get; set; }

    /// <summary>
    /// 反馈列表.
    /// </summary>
    [JsonPropertyName("feedback")]
    public List<LiveFeedback> Feedback { get; set; }

    /// <summary>
    /// 序号.
    /// </summary>
    [JsonPropertyName("index")]
    public long Index { get; set; }

    /// <summary>
    /// 是否隐藏反馈.0-不隐藏，1-隐藏.
    /// </summary>
    [JsonPropertyName("is_hide_feedback")]
    public int IsHideFeedback { get; set; }

    /// <summary>
    /// 封面内容样式.
    /// </summary>
    public class LiveCoverContent
    {
        /// <summary>
        /// 封面文本.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// 反馈.
    /// </summary>
    public class LiveFeedback
    {
        /// <summary>
        /// 标题.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// 副标题.
        /// </summary>
        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        /// <summary>
        /// 类型.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 理由集合.
        /// </summary>
        [JsonPropertyName("reasons")]
        public List<LiveFeedbackReason> Reasons { get; set; }

        /// <summary>
        /// 直播反馈理由.
        /// </summary>
        public class LiveFeedbackReason
        {
            /// <summary>
            /// 标识符.
            /// </summary>
            [JsonPropertyName("id")]
            public long Id { get; set; }

            /// <summary>
            /// 名称.
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// Id类型.
            /// </summary>
            [JsonPropertyName("id_type")]
            public string IdType { get; set; }
        }
    }
}

