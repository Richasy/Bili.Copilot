// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 推荐卡片的定义.
/// </summary>
public class RecommendCard
{
    /// <summary>
    /// 卡片类型.
    /// </summary>
    [JsonPropertyName("card_type")]
    public string CardType { get; set; }

    /// <summary>
    /// 处理卡片的程序.
    /// </summary>
    [JsonPropertyName("card_goto")]
    public string CardGoto { get; set; }

    /// <summary>
    /// 卡片参数.
    /// </summary>
    [JsonPropertyName("args")]
    public RecommendCardArgs CardArgs { get; set; }

    /// <summary>
    /// 偏移值标识符.
    /// </summary>
    [JsonPropertyName("idx")]
    public long Index { get; set; }

    /// <summary>
    /// 上下文菜单项列表.
    /// </summary>
    [JsonPropertyName("three_point_v2")]
    public List<RecommendContextMenuItem> ContextMenuItems { get; set; }

    /// <summary>
    /// 需要播放的类型.
    /// </summary>
    [JsonPropertyName("goto")]
    public string Goto { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 导航地址.
    /// </summary>
    [JsonPropertyName("uri")]
    public string NavigateUri { get; set; }

    /// <summary>
    /// 视频或番剧的Id.
    /// </summary>
    [JsonPropertyName("param")]
    public string Parameter { get; set; }

    /// <summary>
    /// 播放参数.
    /// </summary>
    [JsonPropertyName("player_args")]
    public PlayerArgs PlayerArgs { get; set; }

    /// <summary>
    /// 播放数文本.
    /// </summary>
    [JsonPropertyName("cover_left_text_2")]
    public string PlayCountText { get; set; }

    /// <summary>
    /// 状态副文本，对于视频来说是弹幕数，对于番剧来说是点赞数.
    /// </summary>
    [JsonPropertyName("cover_left_text_3")]
    public string SubStatusText { get; set; }

    /// <summary>
    /// 时长文本.
    /// </summary>
    [JsonPropertyName("cover_left_text_1")]
    public string DurationText { get; set; }

    /// <summary>
    /// 推荐原因.
    /// </summary>
    [JsonPropertyName("top_rcmd_reason")]
    public string RecommendReason { get; set; }

    /// <summary>
    /// 是否可以播放，1-可以.
    /// </summary>
    [JsonPropertyName("can_play")]
    public int CanPlay { get; set; }

    /// <summary>
    /// 说明文本.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// 遮罩内容.
    /// </summary>
    [JsonPropertyName("mask")]
    public RecommendCardMask Mask { get; set; }
}

/// <summary>
/// 推荐卡片的参数.
/// </summary>
public class RecommendCardArgs
{
    /// <summary>
    /// 发布者Id.
    /// </summary>
    [JsonPropertyName("up_id")]
    public int PublisherId { get; set; }

    /// <summary>
    /// 发布者名称.
    /// </summary>
    [JsonPropertyName("up_name")]
    public string PublisherName { get; set; }

    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("rid")]
    public int PartitionId { get; set; }

    /// <summary>
    /// 分区名称.
    /// </summary>
    [JsonPropertyName("rname")]
    public string PartitionName { get; set; }

    /// <summary>
    /// 子分区Id.
    /// </summary>
    [JsonPropertyName("tid")]
    public int SubPartitionId { get; set; }

    /// <summary>
    /// 子分区名称.
    /// </summary>
    [JsonPropertyName("tname")]
    public string SubPartitionName { get; set; }

    /// <summary>
    /// 视频Aid.
    /// </summary>
    [JsonPropertyName("aid")]
    public int Aid { get; set; }
}

/// <summary>
/// 播放器参数.
/// </summary>
public class PlayerArgs
{
    /// <summary>
    /// 视频的Aid.
    /// </summary>
    [JsonPropertyName("aid")]
    public int Aid { get; set; }

    /// <summary>
    /// 视频第一个分P的Id.
    /// </summary>
    [JsonPropertyName("cid")]
    public int Cid { get; set; }

    /// <summary>
    /// 视频类型，一般为av.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 视频时长 (秒).
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
}

/// <summary>
/// 推荐视频的上下文菜单内容.
/// </summary>
public class RecommendContextMenuItem
{
    /// <summary>
    /// 显示标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 菜单项类型。watch_later:稍后再看. feedback:反馈. dislike:不喜欢.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 副标题.
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    /// <summary>
    /// 原因列表.
    /// </summary>
    [JsonPropertyName("reasons")]
    public List<RecommendDislikeReason> Reasons { get; set; }
}

/// <summary>
/// 推荐视频的不喜欢原因.
/// </summary>
public class RecommendDislikeReason
{
    /// <summary>
    /// 原因标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 显示的文本.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 提示文本.
    /// </summary>
    [JsonPropertyName("toast")]
    public string Tip { get; set; }
}

/// <summary>
/// 推荐卡片的遮罩内容.
/// </summary>
public class RecommendCardMask
{
    /// <summary>
    /// 推荐卡片的头像.
    /// </summary>
    [JsonPropertyName("avatar")]
    public RecommendAvatar Avatar { get; set; }
}

/// <summary>
/// 推荐卡片的头像.
/// </summary>
public class RecommendAvatar
{
    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("Text")]
    public string UserName { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("up_id")]
    public int UserId { get; set; }
}

