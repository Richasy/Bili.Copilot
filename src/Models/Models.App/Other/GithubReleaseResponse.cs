// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// GitHub的发布响应结果.
/// </summary>
public class GitHubReleaseResponse
{
    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string Url { get; set; }

    /// <summary>
    /// 版本标签.
    /// </summary>
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 是否为预发布版本.
    /// </summary>
    [JsonPropertyName("prerelease")]
    public bool IsPreRelease { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("published_at")]
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// 发布说明.
    /// </summary>
    [JsonPropertyName("body")]
    public string Description { get; set; }
}

