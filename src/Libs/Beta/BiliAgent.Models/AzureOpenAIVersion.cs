// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BiliAgent.Models;

/// <summary>
/// Azure OpenAI 服务版本.
/// </summary>
[JsonConverter(typeof(AzureOpenAIVersionConverter))]
public enum AzureOpenAIVersion
{
    /// <summary>
    /// Service version "2022-12-01".
    /// </summary>
    V2022_12_01 = 0,

    /// <summary>
    /// Service version "2023-05-15".
    /// </summary>
    V2023_05_15,

    /// <summary>
    /// Service version "2023-06-01-preview".
    /// </summary>
    V2023_06_01_Preview,

    /// <summary>
    /// Service version "2023-10-01-preview".
    /// </summary>
    V2023_10_01_Preview,

    /// <summary>
    /// Service version "2024-02-15-preview".
    /// </summary>
    V2024_02_15_Preview,

    /// <summary>
    /// Service version "2024-03-01-preview".
    /// </summary>
    V2024_03_01_Preview,

    /// <summary>
    /// Service version "2024-02-01".
    /// </summary>
    V2024_02_01,
}

/// <summary>
/// Azure OpenAI 服务版本转换器.
/// </summary>
public sealed class AzureOpenAIVersionConverter : JsonConverter<AzureOpenAIVersion>
{
    /// <inheritdoc/>
    public override AzureOpenAIVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var version = reader.GetString();
        if (string.IsNullOrEmpty(version))
        {
            return AzureOpenAIVersion.V2024_02_01;
        }

        if (version.Equals("2024-02-01"))
        {
            return AzureOpenAIVersion.V2024_02_01;
        }

        if (version.Equals("2024-03-01-preview", StringComparison.OrdinalIgnoreCase))
        {
            return AzureOpenAIVersion.V2024_03_01_Preview;
        }

        if (version.Equals("2024-02-15-preview", StringComparison.OrdinalIgnoreCase))
        {
            return AzureOpenAIVersion.V2024_02_15_Preview;
        }

        if (version.Equals("2023-10-01-preview", StringComparison.OrdinalIgnoreCase))
        {
            return AzureOpenAIVersion.V2023_10_01_Preview;
        }

        if (version.Equals("2023-06-01-preview", StringComparison.OrdinalIgnoreCase))
        {
            return AzureOpenAIVersion.V2023_06_01_Preview;
        }

        if (version.Equals("2023-05-15"))
        {
            return AzureOpenAIVersion.V2023_05_15;
        }

        if (version.Equals("2022-12-01"))
        {
            return AzureOpenAIVersion.V2022_12_01;
        }

        throw new JsonException($"Unexpected Azure OpenAI version: {version}");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, AzureOpenAIVersion value, JsonSerializerOptions options)
    {
        if (value == AzureOpenAIVersion.V2024_02_01)
        {
            writer.WriteStringValue("2024-02-01");
        }
        else if (value == AzureOpenAIVersion.V2024_03_01_Preview)
        {
            writer.WriteStringValue("2024-03-01-preview");
        }
        else if (value == AzureOpenAIVersion.V2024_02_15_Preview)
        {
            writer.WriteStringValue("2024-02-15-preview");
        }
        else if (value == AzureOpenAIVersion.V2023_10_01_Preview)
        {
            writer.WriteStringValue("2023-10-01-preview");
        }
        else if (value == AzureOpenAIVersion.V2023_06_01_Preview)
        {
            writer.WriteStringValue("2023-06-01-preview");
        }
        else if (value == AzureOpenAIVersion.V2023_05_15)
        {
            writer.WriteStringValue("2023-05-15");
        }
        else if (value == AzureOpenAIVersion.V2022_12_01)
        {
            writer.WriteStringValue("2022-12-01");
        }
        else
        {
            throw new JsonException($"Unexpected Azure OpenAI version: {value}");
        }
    }
}
