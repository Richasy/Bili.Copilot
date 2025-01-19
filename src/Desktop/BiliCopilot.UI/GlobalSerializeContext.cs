// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json.Serialization;
using BiliAgent.Models;
using BiliCopilot.UI.Models;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MediaIdentifier))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(PinItem))]
[JsonSerializable(typeof(List<PinItem>))]
[JsonSerializable(typeof(WebDavConfig))]
[JsonSerializable(typeof(List<WebDavConfig>))]
internal sealed partial class GlobalSerializeContext : JsonSerializerContext
{
}
