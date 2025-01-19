// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models;

/// <summary>
/// WebDav 配置.
/// </summary>
public sealed class WebDavConfig
{
    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 配置名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 主机地址.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 路径.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 端口.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 密码.
    /// </summary>
    public string? Password { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is WebDavConfig config && Id == config.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(Host))
        {
            return string.Empty;
        }

        var temp = new Uri(Host);
        var builder = new UriBuilder();
        builder.Host = temp.Host;
        builder.Scheme = temp.Scheme;
        builder.Port = Port ?? 80;
        builder.Path = Path;
        return builder.ToString();
    }

    /// <summary>
    /// 获取服务器地址.
    /// </summary>
    /// <returns>服务器地址.</returns>
    public string GetServer()
    {
        if (string.IsNullOrEmpty(Host))
        {
            return string.Empty;
        }

        var temp = new Uri(Host);
        var builder = new UriBuilder();
        builder.Host = temp.Host;
        builder.Scheme = temp.Scheme;
        builder.Port = Port ?? 80;
        return builder.ToString();
    }
}
