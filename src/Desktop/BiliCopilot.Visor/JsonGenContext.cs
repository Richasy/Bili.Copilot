// Copyright (c) Richasy. All rights reserved.

using BiliCopilot.Visor.Models;
using System.Text.Json.Serialization;

namespace BiliCopilot.Visor;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(VisorCommand))]
[JsonSerializable(typeof(VisorResponse))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
