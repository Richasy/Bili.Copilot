// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili.Others;

/// <summary>
/// 密码加密响应.
/// </summary>
public class PasswordEncryptedResponse
{
    /// <summary>
    /// 哈希.
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    /// <summary>
    /// 密钥.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }
}
