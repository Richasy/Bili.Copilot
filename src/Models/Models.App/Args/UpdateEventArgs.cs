// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.App.Other;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// �汾�����¼�����.
/// </summary>
public class UpdateEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateEventArgs"/> class.
    /// </summary>
    /// <param name="response">��Ӧ���.</param>
    public UpdateEventArgs(GithubReleaseResponse response)
    {
        Version = response.TagName.Replace("v", string.Empty)
            .Replace(".pre-release", string.Empty);
        ReleaseTitle = response.Name;
        ReleaseDescription = response.Description;
        DownloadUrl = new Uri(response.Url);
        PublishTime = response.PublishTime.ToLocalTime();
        IsPreRelease = response.IsPreRelease;
    }

    /// <summary>
    /// �汾��.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// ��������.
    /// </summary>
    public string ReleaseTitle { get; set; }

    /// <summary>
    /// ����˵��.
    /// </summary>
    public string ReleaseDescription { get; set; }

    /// <summary>
    /// ���ص�ַ.
    /// </summary>
    public Uri DownloadUrl { get; set; }

    /// <summary>
    /// ����ʱ��.
    /// </summary>
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// �Ƿ�ΪԤ����.
    /// </summary>
    public bool IsPreRelease { get; set; }
}

