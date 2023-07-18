// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC内容详情.
/// </summary>
public class PgcDisplayInformation
{
    /// <summary>
    /// 演员表.
    /// </summary>
    [JsonPropertyName("actor")]
    public PgcStaff Actor { get; set; }

    /// <summary>
    /// 昵称，假名.
    /// </summary>
    [JsonPropertyName("alias")]
    public string Alias { get; set; }

    /// <summary>
    /// 所属地区.
    /// </summary>
    [JsonPropertyName("areas")]
    public List<PgcArea> Areas { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 动态副标题.
    /// </summary>
    [JsonPropertyName("dynamic_subtitle")]
    public string DynamicSubtitle { get; set; }

    /// <summary>
    /// 评价，说明文本.
    /// </summary>
    [JsonPropertyName("evaluate")]
    public string Evaluate { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 媒体Id.
    /// </summary>
    [JsonPropertyName("media_id")]
    public int MediaId { get; set; }

    /// <summary>
    /// 模式，用途不明.
    /// </summary>
    [JsonPropertyName("mode")]
    public int Mode { get; set; }

    /// <summary>
    /// 模块.
    /// </summary>
    [JsonPropertyName("modules")]
    public List<PgcDetailModule> Modules { get; set; }

    /// <summary>
    /// 原名.
    /// </summary>
    [JsonPropertyName("origin_name")]
    public string OriginName { get; set; }

    /// <summary>
    /// 发布信息.
    /// </summary>
    [JsonPropertyName("publish")]
    public PgcPublishInformation PublishInformation { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// 剧集标题.
    /// </summary>
    [JsonPropertyName("season_title")]
    public string SeasonTitle { get; set; }

    /// <summary>
    /// 系列.
    /// </summary>
    [JsonPropertyName("series")]
    public PgcSeries Series { get; set; }

    /// <summary>
    /// 分享标题.
    /// </summary>
    [JsonPropertyName("share_copy")]
    public string ShareTitle { get; set; }

    /// <summary>
    /// 分享链接.
    /// </summary>
    [JsonPropertyName("share_url")]
    public string ShareUrl { get; set; }

    /// <summary>
    /// 短链接.
    /// </summary>
    [JsonPropertyName("short_link")]
    public string ShortLink { get; set; }

    /// <summary>
    /// 矩形封面.
    /// </summary>
    [JsonPropertyName("square_cover")]
    public string SquareCover { get; set; }

    /// <summary>
    /// 参与人员，制作信息.
    /// </summary>
    [JsonPropertyName("staff")]
    public PgcStaff Staff { get; set; }

    /// <summary>
    /// 详情互动数据（包括投币数，观看数等）.
    /// </summary>
    [JsonPropertyName("stat")]
    public PgcInformationStat InformationStat { get; set; }

    /// <summary>
    /// 状态（数值含义不明）.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 索引筛选列表.
    /// </summary>
    [JsonPropertyName("styles")]
    public List<PgcIndex> IndexList { get; set; }

    /// <summary>
    /// 副标题.
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 总集数.
    /// </summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; set; }

    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 类型说明.
    /// </summary>
    [JsonPropertyName("type_desc")]
    public string TypeDescription { get; set; }

    /// <summary>
    /// 类型名.
    /// </summary>
    [JsonPropertyName("type_name")]
    public string TypeName { get; set; }

    /// <summary>
    /// 用户状态，包括是否已追番及播放进度等.
    /// </summary>
    [JsonPropertyName("user_status")]
    public PgcUserStatus UserStatus { get; set; }

    /// <summary>
    /// 动态标签页.
    /// </summary>
    [JsonPropertyName("activity_tab")]
    public PgcActivityTab ActivityTab { get; set; }

    /// <summary>
    /// 评分.
    /// </summary>
    [JsonPropertyName("rating")]
    public PgcRating Rating { get; set; }

    /// <summary>
    /// 演职人员.
    /// </summary>
    [JsonPropertyName("celebrity")]
    public List<PgcCelebrity> Celebrity { get; set; }

    /// <summary>
    /// 警示信息.
    /// </summary>
    [JsonPropertyName("dialog")]
    public PgcPlayerDialog Warning { get; set; }
}

/// <summary>
/// 播放器警告.
/// </summary>
public class PgcPlayerDialog
{
    /// <summary>
    /// 警告代号.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// 警告信息.
    /// </summary>
    [JsonPropertyName("msg")]
    public string Message { get; set; }

    /// <summary>
    /// 警告类型.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

/// <summary>
/// 发布详情.
/// </summary>
public class PgcPublishInformation
{
    /// <summary>
    /// 是否已完结，0-未完结，1-已完结.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public int IsFinish { get; set; }

    /// <summary>
    /// 是否已开始连载.
    /// </summary>
    [JsonPropertyName("is_started")]
    public int IsStarted { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("pub_time")]
    public string PublishTime { get; set; }

    /// <summary>
    /// 显示的可读发布时间.
    /// </summary>
    [JsonPropertyName("pub_time_show")]
    public string DisplayPublishTime { get; set; }

    /// <summary>
    /// 显示的可读发布日期.
    /// </summary>
    [JsonPropertyName("release_date_show")]
    public string DisplayReleaseDate { get; set; }

    /// <summary>
    /// 显示的连载进度.
    /// </summary>
    [JsonPropertyName("time_length_show")]
    public string DisplayProgress { get; set; }

    /// <summary>
    /// 未知发布时间. 0-已知，1-未知.
    /// </summary>
    [JsonPropertyName("unknow_pub_date")]
    public int UnknowPublishDate { get; set; }
}

/// <summary>
/// PGC单集信息.
/// </summary>
public class PgcEpisodeDetail
{
    /// <summary>
    /// 视频Id.
    /// </summary>
    [JsonPropertyName("aid")]
    public int Aid { get; set; }

    /// <summary>
    /// BV Id.
    /// </summary>
    [JsonPropertyName("bvid")]
    public string BvId { get; set; }

    /// <summary>
    /// 分P Id.
    /// </summary>
    [JsonPropertyName("cid")]
    public int PartId { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 时长.
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// 分集索引.
    /// </summary>
    [JsonPropertyName("ep_index")]
    public int Index { get; set; }

    /// <summary>
    /// 分集Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 长标题.
    /// </summary>
    [JsonPropertyName("long_title")]
    public string LongTitle { get; set; }

    /// <summary>
    /// 发布时间（秒）.
    /// </summary>
    [JsonPropertyName("pub_time")]
    public int PublishTime { get; set; }

    /// <summary>
    /// 是否为PV？ 0-不是，1-是.
    /// </summary>
    [JsonPropertyName("pv")]
    public int IsPV { get; set; }

    /// <summary>
    /// 当前分块内的索引.
    /// </summary>
    [JsonPropertyName("section_index")]
    public int SectionIndex { get; set; }

    /// <summary>
    /// 分享标题.
    /// </summary>
    [JsonPropertyName("share_copy")]
    public string ShareTitle { get; set; }

    /// <summary>
    /// 分享链接.
    /// </summary>
    [JsonPropertyName("share_url")]
    public string ShareUrl { get; set; }

    /// <summary>
    /// 短链接.
    /// </summary>
    [JsonPropertyName("short_link")]
    public string ShortLink { get; set; }

    /// <summary>
    /// 状态.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 副标题.
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 报告信息.
    /// </summary>
    [JsonPropertyName("report")]
    public PgcModuleReport Report { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 单集社区信息.
    /// </summary>
    [JsonPropertyName("stat")]
    public PgcEpisodeStat Stat { get; set; }
}

/// <summary>
/// PGC系列.
/// </summary>
public class PgcSeries
{
    /// <summary>
    /// 系列Id.
    /// </summary>
    [JsonPropertyName("series_id")]
    public int Id { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("series_title")]
    public string Title { get; set; }
}

/// <summary>
/// PGC内容的用户交互数据.
/// </summary>
public class PgcUserStatus
{
    /// <summary>
    /// 是否已关注/追番. 0-没有，1-已追.
    /// </summary>
    [JsonPropertyName("follow")]
    public int IsFollow { get; set; }

    /// <summary>
    /// 付费状态. 0-未付费，1-已付费.
    /// </summary>
    [JsonPropertyName("pay")]
    public int Pay { get; set; }

    /// <summary>
    /// 播放历史记录.
    /// </summary>
    [JsonPropertyName("progress")]
    public PgcProgress Progress { get; set; }

    /// <summary>
    /// 是否需要大会员. 0-不需要，1-需要.
    /// </summary>
    [JsonPropertyName("vip")]
    public int IsVip { get; set; }
}

/// <summary>
/// PGC内容制作人员信息.
/// </summary>
public class PgcStaff
{
    /// <summary>
    /// 内容.
    /// </summary>
    [JsonPropertyName("info")]
    public string Information { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

/// <summary>
/// PGC详情信息的社区数据.
/// </summary>
public class PgcInformationStat
{
    /// <summary>
    /// 投币数.
    /// </summary>
    [JsonPropertyName("coins")]
    public long CoinCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmakus")]
    public long DanmakuCount { get; set; }

    /// <summary>
    /// 单集收藏数.
    /// </summary>
    [JsonPropertyName("favorite")]
    public long FavoriteCount { get; set; }

    /// <summary>
    /// 系列追番/收藏数.
    /// </summary>
    [JsonPropertyName("favorites")]
    public long SeriesFavoriteCount { get; set; }

    /// <summary>
    /// 追番/收藏显示文本.
    /// </summary>
    [JsonPropertyName("followers")]
    public string FollowerDisplayText { get; set; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    [JsonPropertyName("likes")]
    public long LikeCount { get; set; }

    /// <summary>
    /// 播放量显示文本.
    /// </summary>
    [JsonPropertyName("play")]
    public string PlayDisplayText { get; set; }

    /// <summary>
    /// 回复数.
    /// </summary>
    [JsonPropertyName("reply")]
    public long ReplyCount { get; set; }

    /// <summary>
    /// 分享数.
    /// </summary>
    [JsonPropertyName("share")]
    public long ShareCount { get; set; }

    /// <summary>
    /// 播放次数.
    /// </summary>
    [JsonPropertyName("views")]
    public long PlayCount { get; set; }
}

/// <summary>
/// PGC单集的社区数据.
/// </summary>
public class PgcEpisodeStat
{
    /// <summary>
    /// 投币数.
    /// </summary>
    [JsonPropertyName("coins")]
    public long CoinCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmakus")]
    public long DanmakuCount { get; set; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    [JsonPropertyName("likes")]
    public long LikeCount { get; set; }

    /// <summary>
    /// 播放量.
    /// </summary>
    [JsonPropertyName("play")]
    public long PlayCount { get; set; }

    /// <summary>
    /// 回复数.
    /// </summary>
    [JsonPropertyName("reply")]
    public long ReplyCount { get; set; }
}

/// <summary>
/// PGC内容历史记录.
/// </summary>
public class PgcProgress
{
    /// <summary>
    /// 最后一次播放的单集Id.
    /// </summary>
    [JsonPropertyName("last_ep_id")]
    public int LastEpisodeId { get; set; }

    /// <summary>
    /// 最后一次播放的单集索引.
    /// </summary>
    [JsonPropertyName("last_ep_index")]
    public string LastEpisodeIndex { get; set; }

    /// <summary>
    /// 播放进度（秒）.
    /// </summary>
    [JsonPropertyName("last_time")]
    public int LastTime { get; set; }
}

/// <summary>
/// PGC动态标签.
/// </summary>
public class PgcActivityTab
{
    /// <summary>
    /// 标签Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [JsonPropertyName("show_name")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 全称.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 类别（目前仅处理101）.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}

/// <summary>
/// PGC内容评分.
/// </summary>
public class PgcRating
{
    /// <summary>
    /// 评分人数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 综合评分.
    /// </summary>
    [JsonPropertyName("score")]
    public long Score { get; set; }
}

/// <summary>
/// PGC内容所属地区.
/// </summary>
public class PgcArea
{
    /// <summary>
    /// 地区Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 属地名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

/// <summary>
/// PGC详情的模块.
/// </summary>
public class PgcDetailModule
{
    /// <summary>
    /// 数据.
    /// </summary>
    [JsonPropertyName("data")]
    public PgcDetailModuleData Data { get; set; }

    /// <summary>
    /// 未知发布时间. 0-已知，1-未知.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 样式.
    /// </summary>
    [JsonPropertyName("style")]
    public string Style { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

/// <summary>
/// PGC详情模块数据.
/// </summary>
public class PgcDetailModuleData
{
    /// <summary>
    /// 剧集列表.
    /// </summary>
    [JsonPropertyName("episodes")]
    public List<PgcEpisodeDetail> Episodes { get; set; }

    /// <summary>
    /// 关联系列.
    /// </summary>
    [JsonPropertyName("seasons")]
    public List<PgcSeason> Seasons { get; set; }

    /// <summary>
    /// 模块Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
}

/// <summary>
/// PGC剧集系列.
/// </summary>
public class PgcSeason
{
    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string Badge { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 是否是最新. 0-不是，1-是.
    /// </summary>
    [JsonPropertyName("is_new")]
    public int IsNew { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 最新内容.
    /// </summary>
    [JsonPropertyName("new_ep")]
    public PgcEpisode NewEpisode { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// 剧集标题.
    /// </summary>
    [JsonPropertyName("season_title")]
    public string SeasonTitle { get; set; }

    /// <summary>
    /// 标题全称.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

/// <summary>
/// PGC关联索引.
/// </summary>
public class PgcIndex
{
    /// <summary>
    /// Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 导航地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

/// <summary>
/// 演职人员.
/// </summary>
public class PgcCelebrity
{
    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }

    /// <summary>
    /// 说明.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// 演职员Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 演职员姓名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 简短介绍.
    /// </summary>
    [JsonPropertyName("short_desc")]
    public string ShortDescription { get; set; }
}

/// <summary>
/// PGC模块报告数据.
/// </summary>
public class PgcModuleReport
{
    /// <summary>
    /// Aid.
    /// </summary>
    [JsonPropertyName("aid")]
    public string Aid { get; set; }

    /// <summary>
    /// 分集标题.
    /// </summary>
    [JsonPropertyName("ep_title")]
    public string EpisodeTitle { get; set; }

    /// <summary>
    /// 分集Id.
    /// </summary>
    [JsonPropertyName("epid")]
    public string EpisodeId { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public string SeasonId { get; set; }

    /// <summary>
    /// 剧集类型.
    /// </summary>
    [JsonPropertyName("season_type")]
    public string SeasonType { get; set; }

    /// <summary>
    /// 分块Id.
    /// </summary>
    [JsonPropertyName("section_id")]
    public string SectionId { get; set; }

    /// <summary>
    /// 分块类型.
    /// </summary>
    [JsonPropertyName("section_type")]
    public string SectionType { get; set; }
}

