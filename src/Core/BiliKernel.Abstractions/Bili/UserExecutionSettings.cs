// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Bili;

/// <summary>
/// 用于对用户相关的请求进行设置的类.
/// </summary>
public class UserExecutionSettings
{
    private string? _userId;
    private IDictionary<string, object>? _extensionData;

    /// <summary>
    /// 用户 ID.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string? UserId
    {
        get => _userId;

        set
        {
            ThrowIfFrozen();
            _userId = value;
        }
    }

    /// <summary>
    /// 附加属性，包含序列化的结构数据.
    /// </summary>
    /// <remarks>
    /// 尽量避免直接使用此属性，而是派生 <see cref="UserExecutionSettings"/>.
    /// </remarks>
    [JsonExtensionData]
    public IDictionary<string, object>? ExtensionData
    {
        get => _extensionData;

        set
        {
            ThrowIfFrozen();
            _extensionData = value;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="UserExecutionSettings"/> are currently modifiable.
    /// </summary>
    [JsonIgnore]
    public bool IsFrozen { get; private set; }

    /// <summary>
    /// Makes the current <see cref="UserExecutionSettings"/> unmodifiable and sets its IsFrozen property to true.
    /// </summary>
    public virtual void Freeze()
    {
        if (IsFrozen)
        {
            return;
        }

        IsFrozen = true;

        if (_extensionData is not null)
        {
            _extensionData = new ReadOnlyDictionary<string, object>(_extensionData);
        }
    }

    /// <summary>
    /// Creates a new <see cref="UserExecutionSettings"/> object that is a copy of the current instance.
    /// </summary>
    public virtual UserExecutionSettings Clone()
    {
        return new()
        {
            UserId = UserId,
            ExtensionData = ExtensionData is not null ? new Dictionary<string, object>(ExtensionData) : null
        };
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the <see cref="UserExecutionSettings"/> are frozen.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    protected void ThrowIfFrozen()
    {
        if (IsFrozen)
        {
            throw new InvalidOperationException("UserExecutionSettings are frozen and cannot be modified.");
        }
    }
}
