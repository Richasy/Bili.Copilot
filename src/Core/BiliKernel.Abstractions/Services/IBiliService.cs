// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Services;

/// <summary>
/// Represents an Bili service.
/// </summary>
public interface IBiliService
{
    /// <summary>
    /// Gets the Bili service attributes.
    /// </summary>
    IReadOnlyDictionary<string, object?> Attributes { get; }
}
