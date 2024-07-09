// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class PgcIndexConditionResponse
{
    [JsonPropertyName("filter")]
    public IList<PgcIndexFilter>? FilterList { get; set; }

    [JsonPropertyName("order")]
    public IList<PgcIndexOrder>? OrderList { get; set; }
}

internal sealed class PgcIndexFilter
{
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("values")]
    public IList<PgcIndexFilterValue>? Values { get; set; }
}

internal sealed class PgcIndexFilterValue
{
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

internal sealed class PgcIndexOrder
{
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
