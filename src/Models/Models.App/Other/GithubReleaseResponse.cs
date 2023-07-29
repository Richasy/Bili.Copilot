// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// Github�ķ�����Ӧ���.
/// </summary>
public class GithubReleaseResponse
{
    /// <summary>
    /// ��ַ.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string Url { get; set; }

    /// <summary>
    /// �汾��ǩ.
    /// </summary>
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// �Ƿ�ΪԤ�����汾.
    /// </summary>
    [JsonPropertyName("prerelease")]
    public bool IsPreRelease { get; set; }

    /// <summary>
    /// ����ʱ��.
    /// </summary>
    [JsonPropertyName("published_at")]
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// ����˵��.
    /// </summary>
    [JsonPropertyName("body")]
    public string Description { get; set; }
}

