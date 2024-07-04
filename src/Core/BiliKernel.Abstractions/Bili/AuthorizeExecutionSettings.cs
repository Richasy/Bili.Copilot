// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Bili;

/// <summary>
/// 授权执行设置.
/// </summary>
public class AuthorizeExecutionSettings
{
    private IDictionary<string, object>? _extensionData;

    /// <summary>
    /// 附加属性，包含序列化的结构数据.
    /// </summary>
    /// <remarks>
    /// 尽量避免直接使用此属性，而是派生 <see cref="AuthorizeExecutionSettings"/>.
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
    /// Gets a value that indicates whether the <see cref="AuthorizeExecutionSettings"/> are currently modifiable.
    /// </summary>
    [JsonIgnore]
    public bool IsFrozen { get; private set; }

    /// <summary>
    /// Makes the current <see cref="AuthorizeExecutionSettings"/> unmodifiable and sets its IsFrozen property to true.
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
    /// Throws an exception if the <see cref="AuthorizeExecutionSettings"/> are currently unmodifiable.
    /// </summary>
    protected void ThrowIfFrozen()
    {
        if (IsFrozen)
        {
            throw new InvalidOperationException("The settings are frozen and cannot be modified.");
        }
    }

    /// <summary>
    /// Creates a new <see cref="AuthorizeExecutionSettings"/> object that is a copy of the current instance.
    /// </summary>
    public virtual AuthorizeExecutionSettings Clone()
    {
        return new()
        {
            ExtensionData = ExtensionData is not null ? new Dictionary<string, object>(ExtensionData) : null
        };
    }
}
